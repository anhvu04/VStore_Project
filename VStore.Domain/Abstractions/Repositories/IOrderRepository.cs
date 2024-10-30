using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IOrderRepository : IRepositoryBase<Order, Guid>
{
}