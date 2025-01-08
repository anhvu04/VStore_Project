using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Abstractions.RedisCartService;
using VStore.Application.Abstractions.VNPayService;
using VStore.Application.Models.PayOsService;
using VStore.Application.Models.VnPayService;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Checkout.Command.RedisCartCheckout;

public class RedisCartCheckoutCommandHandler : ICommandHandler<RedisCartCheckoutCommand, CheckoutResponseModel>
{
    private readonly IRedisCartService _redisCartService;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVnPayService _vnPayService;
    private readonly IPayOsService _payOsService;

    public RedisCartCheckoutCommandHandler(IRedisCartService redisCartService, IProductRepository productRepository,
        ICustomerAddressRepository customerAddressRepository, IOrderRepository orderRepository,
        IOrderDetailRepository orderDetailRepository, IUnitOfWork unitOfWork, IVnPayService vnPayService,
        IPayOsService payOsService)
    {
        _redisCartService = redisCartService;
        _productRepository = productRepository;
        _customerAddressRepository = customerAddressRepository;
        _orderRepository = orderRepository;
        _orderDetailRepository = orderDetailRepository;
        _unitOfWork = unitOfWork;
        _vnPayService = vnPayService;
        _payOsService = payOsService;
    }

    public async Task<Result<CheckoutResponseModel>> Handle(RedisCartCheckoutCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await _redisCartService.GetCartAsync(request.CartId);
        if (cart is null || cart.RedisCartItems.Count == 0)
        {
            return Result<CheckoutResponseModel>.Failure(DomainError.Cart.CartNotFoundOrEmpty);
        }

        var orderDetailList = new List<OrderDetail>();
        int totalPrice = 0;
        int totalGram = 0;
        foreach (var p in cart.RedisCartItems)
        {
            var product = await _productRepository.FindByIdAsync(p.ProductId, cancellationToken);
            if (product is null)
            {
                return Result<CheckoutResponseModel>.Failure(DomainError.RedisCart.NotExistProduct);
            }

            if (product.Quantity < p.Quantity || product.Status == ProductStatus.OutOfStock)
            {
                return Result<CheckoutResponseModel>.Failure(
                    DomainError.Cart.ProductOutOfStock(new List<string>()
                    {
                        p.ProductId.ToString()
                    }));
            }

            totalPrice += product.SalePrice == 0 ? product.OriginalPrice + p.Quantity : product.SalePrice + p.Quantity;
            totalGram += product.Gram;

            // Add to order detail list
            orderDetailList.Add(new OrderDetail
            {
                Quantity = p.Quantity,
                UnitPrice = product.SalePrice == 0 ? product.OriginalPrice : product.SalePrice,
                ItemPrice =
                    product.SalePrice == 0 ? product.OriginalPrice * p.Quantity : product.SalePrice * p.Quantity,
                ProductName = product.Name,
                Thumbnail = product.Thumbnail,
                ProductId = product.Id,
            });

            //update product quantity
            product.Quantity -= p.Quantity;
            //update status if quantity = 0
            if (product.Quantity == 0)
            {
                product.Status = ProductStatus.OutOfStock;
            }

            _productRepository.Update(product);
        }

        var customerAddress = await
            _customerAddressRepository
                .FindAll(x => x.CustomerId.ToString() == request.CartId && x.Id == request.AddressId, x => x.Customer)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (customerAddress == null)
        {
            return Result<CheckoutResponseModel>.Failure(DomainError.CommonError.NotFound(nameof(CustomerAddress)));
        }

        //add to order
        var order = new Domain.Entities.Order
        {
            TotalPrice = totalPrice,
            ShippingFee = request.ShippingFee,
            TotalAmount = totalPrice + request.ShippingFee,
            TotalGram = totalGram,
            Note = request.Note,
            ReceiverName = customerAddress.ReceiverName,
            PhoneNumber = customerAddress.PhoneNumber,
            Email = customerAddress.Customer.Email,
            Address = customerAddress.Address + ", " + customerAddress.WardName + ", " + customerAddress.DistrictName +
                      ", " + customerAddress.ProvinceName,
            ProvinceId = customerAddress.ProvinceId,
            DistrictId = customerAddress.DistrictId,
            WardCode = customerAddress.WardCode,
            PaymentMethod = (PaymentMethod)request.PaymentMethod,
            TransactionCode = await GenerateTransactionCode(),
            CustomerId = Guid.Parse(request.CartId),
            Status = OrderStatus.Pending
        };
        _orderRepository.Add(order);

        //add to order log
        order.OrderLogs.Add(new OrderLog
        {
            // OrderId = order.Id,
            Status = OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow
        });

        //add to order detail
        foreach (var detail in orderDetailList)
        {
            detail.OrderId = order.Id;
        }

        _orderDetailRepository.AddRange(orderDetailList);

        //remove from cart
        await _redisCartService.DeleteCartAsync(request.CartId);

        var res = new Result<CheckoutResponseModel>(null, true, null);

        // Handle payment by PayOs
        if (request.PaymentMethod == (int)PaymentMethod.Payos)
        {
            res = await HandlePayOsPayment(order, orderDetailList);
        }

        if (request.PaymentMethod == (int)PaymentMethod.Vnpay)
        {
            res = HandleVnPayPayment(request.HttpContext!, order);
        }

        // if no error, save changes    
        if (res.Error == null)
        {
            await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        }

        return res;
    }

    /// <summary>
    /// handle payment by PayOs
    /// </summary>
    /// <param name="order"></param>
    /// <param name="orderDetail"></param>
    /// <returns></returns>
    private async Task<Result<CheckoutResponseModel>> HandlePayOsPayment(Domain.Entities.Order order,
        List<OrderDetail> orderDetail)
    {
        var payment = await _payOsService.CreatePaymentLink(new CreatePayOsPaymentModel
        {
            OrderCode = (long)order.TransactionCode!,
            Amount = order.TotalAmount,
            Description = $"{order.TransactionCode} | Shipfee: {order.ShippingFee}đ",
            Items = orderDetail.Select(x => new ItemData(x.ProductName, x.Quantity, x.ItemPrice)).ToList()
        });
        if (payment.Code != 200)
        {
            return Result<CheckoutResponseModel>.Failure(DomainError.Checkout.PayOsError);
        }

        return Result<CheckoutResponseModel>.Success(new CheckoutResponseModel
        {
            CheckoutUrl = payment.Data!.ToString()!
        });
    }

    /// <summary>
    /// handle payment by VnPay
    /// </summary>
    /// <param name="context"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    private Result<CheckoutResponseModel> HandleVnPayPayment(HttpContext context, Domain.Entities.Order order)
    {
        var vnpayModel = new CreateVnPayPaymentModel
        {
            TxnRef = (long)order.TransactionCode!,
            Amount = order.TotalAmount,
            OrderInfo = $"{order.TransactionCode} | Shipfee: {order.ShippingFee}đ",
            OrderType = "other",
            IpAddr = context.Connection.RemoteIpAddress?.ToString()!
        };

        var vnpayUrl = _vnPayService.CreatePaymentLink(vnpayModel);
        if (vnpayUrl.Code != 200)
        {
            return Result<CheckoutResponseModel>.Failure(DomainError.Checkout.VnPayError);
        }

        return Result<CheckoutResponseModel>.Success(new CheckoutResponseModel
        {
            CheckoutUrl = vnpayUrl.Data!.ToString()!
        });
    }

    /// <summary>
    /// generate transaction code for order
    /// </summary>
    /// <returns></returns>
    private async Task<int> GenerateTransactionCode()
    {
        Random random = new Random();
        int orderCode;
        do
        {
            orderCode = random.Next(0, 10000000);
        } while (await _orderRepository.AnyAsync(x => x.TransactionCode == orderCode));

        return orderCode;
    }
}