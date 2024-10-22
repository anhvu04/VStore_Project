using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IProductRepository : IRepositoryBase<Product, Guid>
{
}