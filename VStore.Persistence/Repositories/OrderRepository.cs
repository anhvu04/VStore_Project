using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class OrderRepository : RepositoryBase<Order, Guid>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }
}