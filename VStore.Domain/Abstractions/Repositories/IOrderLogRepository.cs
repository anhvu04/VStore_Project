using VStore.Domain.Abstractions.Entities;
using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IOrderLogRepository : IRepositoryBase<OrderLog, Guid>
{
    
}