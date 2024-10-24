using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Cart.Command.RemoveFromCart;

public record RemoveFromCartCommand([property: JsonIgnore] Guid UserId) : ICommand
{
    public List<Guid> ProductIds { get; set; } = [];
}