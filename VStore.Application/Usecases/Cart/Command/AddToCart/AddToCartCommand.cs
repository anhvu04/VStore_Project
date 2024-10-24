using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Cart.Common;

namespace VStore.Application.Usecases.Cart.Command.AddToCart;

public record AddToCartCommand([property: JsonIgnore] Guid UserId) : ICommand
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}