using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Checkout.Common;

namespace VStore.Application.Usecases.Checkout.Command.Checkout;

public record CheckoutCommand : ICommand<CheckoutResponseModel>
{
    [JsonIgnore] public Guid UserId { get; init; }
    public Guid AddressId { get; init; }
    public int PaymentMethod { get; init; } = 1;
    public int ShippingFee { get; init; }
    public string? Note { get; init; }
}