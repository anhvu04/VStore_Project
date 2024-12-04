using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class ProductImageRepository : RepositoryBase<ProductImage, int>, IProductImageRepository
{
    public ProductImageRepository(ApplicationDbContext context) : base(context)
    {
    }
}