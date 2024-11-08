using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Order.Common;

namespace VStore.Application.Usecases.Order.Query.GetOrder;

public class GetOrderQuery(Guid orderId, Guid customerId) : IQuery<OrderDetailModel>
{
    public Guid OrderId { get; set; } = orderId;
    [JsonIgnore] public Guid CustomerId { get; set; } = customerId;
}