using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class OrderLogRepository : RepositoryBase<OrderLog, Guid>, IOrderLogRepository
{
    public OrderLogRepository(ApplicationDbContext context) : base(context)
    {
    }
}