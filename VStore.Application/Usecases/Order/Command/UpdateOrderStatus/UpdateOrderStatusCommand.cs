using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Order.Command.UpdateOrderStatus;

public record UpdateOrderStatusCommand([property: JsonIgnore] Guid OrderId) : ICommand
{
    public int? OrderStatus { get; init; }
}