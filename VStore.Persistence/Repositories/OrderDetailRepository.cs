using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class OrderDetailRepository : RepositoryBase<OrderDetail, Guid>, IOrderDetailRepository
{
    public OrderDetailRepository(ApplicationDbContext context) : base(context)
    {
    }
}