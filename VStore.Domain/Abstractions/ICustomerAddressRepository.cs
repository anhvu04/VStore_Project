using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions;

public interface ICustomerAddressRepository : IRepositoryBase<CustomerAddress, Guid>
{
}