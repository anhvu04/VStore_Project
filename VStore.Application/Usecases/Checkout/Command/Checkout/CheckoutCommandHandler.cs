using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Abstractions.VNPayService;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Checkout.Command.Checkout;

public class CheckoutCommandHandler : ICommandHandler<CheckoutCommand, CheckoutResponseModel>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly ICartDetailRepository _cartDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPayOsService _payOsService;
    private readonly IVnPayService _vnPayService;

    public CheckoutCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork, IMapper mapper,
        ICustomerAddressRepository customerAddressRepository, IOrderRepository orderRepository,
        IOrderDetailRepository orderDetailRepository, ICartDetailRepository cartDetailRepository,
        IProductRepository productRepository, IPayOsService payOsService, IVnPayService vnPayService)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _customerAddressRepository = customerAddressRepository;
        _orderRepository = orderRepository;
        _orderDetailRepository = orderDetailRepository;
        _cartDetailRepository = cartDetailRepository;
        _productRepository = productRepository;
        _payOsService = payOsService;
        _vnPayService = vnPayService;
    }

    public async Task<Result<CheckoutResponseModel>> Handle(CheckoutCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.FindAll(x => x.CustomerId == request.UserId).Include(x => x.CartDetails)
            .ThenInclude(x => x.Product)
            .Select(x => new
            {
                CartDetail = x.CartDetails,
                OrderDetail = x.CartDetails.Select(cd => _mapper.Map<OrderDetailModel>(cd)).ToList(),
                TotalGram = x.CartDetails.Sum(cd => cd.Product.Gram * cd.Quantity),
                TotalPrice = x.CartDetails.Sum(cd => cd.Product.SalePrice == 0
                    ? cd.Product.OriginalPrice * cd.Quantity
                    : cd.Product.SalePrice * cd.Quantity),
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (cart == null || cart.OrderDetail.Count == 0)
        {
            return Result<CheckoutResponseModel>.Failure(DomainError.Cart.CartNotFoundOrEmpty);
        }

        var unavailableProducts = cart.OrderDetail.Where(x => x.ProductQuantity < x.Quantity)
            .Select(x => x.ProductName).ToList();
        if (unavailableProducts.Count > 0)
        {
            return Result<CheckoutResponseModel>.Failure(DomainError.Cart.ProductOutOfStock(unavailableProducts));
        }

        var customerAddress = await
            _customerAddressRepository
                .FindAll(x => x.CustomerId == request.UserId && x.Id == request.AddressId, x => x.Customer)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (customerAddress == null)
        {
            return Result<CheckoutResponseModel>.Failure(DomainError.CommonError.NotFound(nameof(CustomerAddress)));
        }

        //add to order
        var order = new Domain.Entities.Order
        {
            TotalPrice = cart.TotalPrice,
            ShippingFee = request.ShippingFee,
            TotalAmount = cart.TotalPrice + request.ShippingFee,
            TotalGram = cart.TotalGram,
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
            CustomerId = request.UserId,
            Status = OrderStatus.Pending
        };
        _orderRepository.Add(order);

        //add to order detail
        var orderDetail = cart.OrderDetail.Select(x => _mapper.Map<OrderDetail>(x)).ToList();
        foreach (var detail in orderDetail)
        {
            detail.OrderId = order.Id;
        }

        _orderDetailRepository.AddRange(orderDetail);

        //remove from cart
        _cartDetailRepository.RemoveRange(cart.CartDetail);

        //update product quantity
        foreach (var product in cart.CartDetail)
        {
            product.Product.Quantity -= product.Quantity;
            //update status if quantity = 0
            if (product.Product.Quantity == 0)
            {
                product.Product.Status = ProductStatus.OutOfStock;
            }

            _productRepository.Update(product.Product);
        }

        var res = new Result<CheckoutResponseModel>(null, true, null);

        // Handle payment by PayOs
        if (request.PaymentMethod == (int)PaymentMethod.Payos)
        {
            res = await HandlePayOsPayment(order, orderDetail);
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


    #region Helper methods

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

    #endregion
}