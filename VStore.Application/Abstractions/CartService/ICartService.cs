using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.CartService;

public interface ICartService
{
    Task<bool> RefreshCartAsync(Guid userId, CancellationToken cancellationToken);
}