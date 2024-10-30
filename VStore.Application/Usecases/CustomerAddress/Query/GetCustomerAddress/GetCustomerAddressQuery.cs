using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.CustomerAddress.Common;

namespace VStore.Application.Usecases.CustomerAddress.Query.GetCustomerAddress;

public record GetCustomerAddressQuery([property: JsonIgnore] Guid UserId, Guid AddressId) : IQuery<CustomerAddressModel>
{
}