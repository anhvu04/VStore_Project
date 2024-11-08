using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Order.Command.CancelOrderCustomer;

public record CancelOrderCustomerCommand : ICommand
{
    [JsonIgnore] public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
}