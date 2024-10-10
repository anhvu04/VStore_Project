using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IUserRepository : IRepositoryBase<User, Guid>
{
}