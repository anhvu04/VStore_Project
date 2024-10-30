using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.CustomerAddress.Command.DeleteCustomerAddress;

public record DeleteCustomerAddressCommand(Guid UserId, Guid AddressId) : ICommand
{
}