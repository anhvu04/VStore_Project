using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.QueryModel;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Order.Common;

namespace VStore.Application.Usecases.Order.Query.GetOrders;

public class GetOrdersQuery
    : PageModel, ISearchModel, IQuery<PageList<OrderModel>>
{
    public GetOrdersQuery()
    {
    }

    [BindNever] public Guid CustomerId { get; set; }
    public string? SearchTerm { get; set; }
    public int? OrderStatus { get; set; }
    public int? PaymentMethod { get; set; }
}