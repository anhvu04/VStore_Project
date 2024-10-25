using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.CustomerAddress.Common;

namespace VStore.Application.Usecases.CustomerAddress.Command.CreateCustomerAddress;

public record CreateCustomerAddressCommand([property: JsonIgnore] Guid UserId) : CustomerAddressModel, ICommand
{
    [JsonIgnore] public new Guid Id { get; init; }
}