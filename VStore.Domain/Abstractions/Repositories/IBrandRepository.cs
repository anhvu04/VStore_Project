using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IBrandRepository : IRepositoryBase<Brand, int>
{
    Task<bool> IsBrandHasProductAsync(int brandId, CancellationToken cancellationToken = default);
}