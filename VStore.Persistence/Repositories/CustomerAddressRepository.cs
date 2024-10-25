using VStore.Domain.Abstractions;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class CustomerAddressRepository : RepositoryBase<CustomerAddress, Guid>, ICustomerAddressRepository
{
    public CustomerAddressRepository(ApplicationDbContext context) : base(context)
    {
    }
}