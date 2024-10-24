using Microsoft.AspNetCore.Mvc.ModelBinding;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.QueryModel;
using VStore.Application.CoreHelper;
using VStore.Application.Usecases.Cart.Common;

namespace VStore.Application.Usecases.Cart.Query.GetCartQuery;

public class GetCartQuery : PageModel, IQuery<CartModel>
{
    [BindNever] public Guid UserId { get; set; }
}