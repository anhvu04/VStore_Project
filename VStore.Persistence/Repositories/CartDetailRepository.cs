using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class CartDetailRepository : RepositoryBase<CartDetail, Guid>, ICartDetailRepository
{
    public CartDetailRepository(ApplicationDbContext context) : base(context)
    {
    }
}