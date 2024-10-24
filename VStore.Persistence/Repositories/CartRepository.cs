using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class CartRepository : RepositoryBase<Cart, Guid>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context)
    {
    }
}