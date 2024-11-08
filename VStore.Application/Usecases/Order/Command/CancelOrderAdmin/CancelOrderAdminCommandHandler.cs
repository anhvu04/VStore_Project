using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Abstractions.VNPayService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Order.Command.CancelOrderAdmin;

public class CancelOrderAdminCommandHandler : ICommandHandler<CancelOrderAdminCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPayOsService _payOsService;
    private readonly IProductRepository _productRepository;
    private readonly IVnPayService _vnPayService;

    public CancelOrderAdminCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork,
        IPayOsService payOsService, IProductRepository productRepository, IVnPayService vnPayService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _payOsService = payOsService;
        _productRepository = productRepository;
        _vnPayService = vnPayService;
    }

    public async Task<Result> Handle(CancelOrderAdminCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.FindAll(x => x.Id == request.OrderId)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(cancellationToken);
        if (order == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Order)));
        }

        if (order.Status == OrderStatus.Cancelled ||
            order.Status == OrderStatus.Delivered ||
            order.Status == OrderStatus.Shipping)
        {
            return Result.Failure(DomainError.Order.OrderCannotBeCancelledByAdmin);
        }

        switch (order)
        {
            case { PaymentMethod: PaymentMethod.Vnpay, Status: OrderStatus.Pending }:
                return Result.Failure(DomainError.Order.VnPayOrderCancelError);
            case { PaymentMethod: PaymentMethod.Payos, Status: OrderStatus.Pending }:
            {
                var result = await _payOsService.CancelPaymentLink((long)order.TransactionCode!);
                if (!result.IsSuccess)
                {
                    return result;
                }

                break;
            }
        }

        order.Status = OrderStatus.Cancelled;
        foreach (var orderDetail in order.OrderDetails)
        {
            if (orderDetail.Product.Status == ProductStatus.OutOfStock)
            {
                orderDetail.Product.Status = ProductStatus.Selling;
            }

            orderDetail.Product.Quantity += orderDetail.Quantity;
            _productRepository.Update(orderDetail.Product);
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(false, true, cancellationToken);
        return Result.Success();
    }
}