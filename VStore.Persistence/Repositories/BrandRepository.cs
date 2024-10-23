using Microsoft.EntityFrameworkCore;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class BrandRepository : RepositoryBase<Brand, int>, IBrandRepository
{
    private readonly ApplicationDbContext _context;

    public BrandRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsBrandHasProductAsync(int brandId, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AnyAsync(x => x.BrandId == brandId, cancellationToken);
    }
}