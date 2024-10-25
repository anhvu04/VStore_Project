using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.CustomerAddress.Common;

namespace VStore.Application.Usecases.CustomerAddress.Command.CreateCustomerAddress;

public record CreateCustomerAddressAddressCommand([property: JsonIgnore] Guid UserId) : CustomerAddressModel, ICommand
{
}