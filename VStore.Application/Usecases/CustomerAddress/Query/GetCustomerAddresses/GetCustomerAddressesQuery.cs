using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.CustomerAddress.Common;

namespace VStore.Application.Usecases.CustomerAddress.Query.GetCustomerAddresses;

public record GetCustomerAddressesQuery([property: JsonIgnore] Guid UserId) : IQuery<List<CustomerAddressModel>>
{
}