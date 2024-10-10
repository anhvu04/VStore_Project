using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface ICustomerRepository : IRepositoryBase<Customer, Guid>
{
}