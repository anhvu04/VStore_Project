using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.PayOsService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Order.Command.CancelOrderCustomer;

public class CancelOrderCustomerCommandHandler : ICommandHandler<CancelOrderCustomerCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IPayOsService _payOsService;

    public CancelOrderCustomerCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork,
        IProductRepository productRepository, IPayOsService payOsService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _payOsService = payOsService;
    }

    public async Task<Result> Handle(CancelOrderCustomerCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.FindAll(x => x.Id == request.OrderId && x.CustomerId == request.CustomerId)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(cancellationToken);
        if (order == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Order)));
        }

        if (order.Status != OrderStatus.Pending)
        {
            return Result.Failure(DomainError.Order.OrderCannotBeCancelledByUser);
        }

        switch (order)
        {
            case { Status: OrderStatus.Pending, PaymentMethod: PaymentMethod.Vnpay }:
                return Result.Failure(DomainError.Order.VnPayOrderCancelError);
            case { Status: OrderStatus.Pending, PaymentMethod: PaymentMethod.Payos }:
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