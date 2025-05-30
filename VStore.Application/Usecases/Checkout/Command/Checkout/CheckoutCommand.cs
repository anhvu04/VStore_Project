using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Checkout.Common;

namespace VStore.Application.Usecases.Checkout.Command.Checkout;

public record CheckoutCommand : ICommand<CheckoutResponseModel>
{
    [JsonIgnore] public Guid UserId { get; init; }
    [JsonIgnore] public HttpContext? HttpContext { get; init; }
    public Guid AddressId { get; init; }
    public int PaymentMethod { get; init; } = (int)Domain.Enums.PaymentMethod.Cod;
    public int ShippingFee { get; init; }
    public Guid VoucherId { get; init; }
    public string? Note { get; init; }
}