using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken, Guid>
{
}