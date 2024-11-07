using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.GhnService;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Models.GhnService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Order.Command.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGhnService _ghnService;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork,
        IGhnService ghnService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _ghnService = ghnService;
    }

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.FindAll(x => x.Id == request.OrderId)
            .Include(x => x.OrderDetails)
            .FirstOrDefaultAsync(cancellationToken);
        if (order == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Order)));
        }

        if (request.OrderStatus.HasValue && order.Status != (OrderStatus)request.OrderStatus)
        {
            if ((int)request.OrderStatus - (int)order.Status != 1)
            {
                return Result.Failure(DomainError.Order.OrderStatusMustBeIncreased);
            }

            if (request.OrderStatus == (int)OrderStatus.Shipping)
            {
                if (order.ShippingCode != null)
                {
                    return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(order.ShippingCode)));
                }

                var result = await _ghnService.CreateShippingOrder(new CreateGhnOrderModel
                {
                    ToName = order.ReceiverName,
                    ToPhone = order.PhoneNumber,
                    ToAddress = order.Address,
                    ToWardCode = order.WardCode,
                    ToDistrictId = order.DistrictId,
                    Weight = order.TotalGram,
                    PaymentTypeId = order.PaymentMethod == PaymentMethod.Cod ? 2 : 1,
                    Note = order.Note,
                    Items = order.OrderDetails.Select(x => new Items
                    {
                        ProductName = x.ProductName,
                        Quantity = x.Quantity
                    }).ToList()
                });
                if (result.Code != 200)
                {
                    return Result.Failure(DomainError.Order.GhnServiceError);
                }

                order.ShippingCode = result.Data!.ToString();
            }

            order.Status = (OrderStatus)request.OrderStatus;
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(false, true, cancellationToken);
        }

        return Result.Success();
    }
}