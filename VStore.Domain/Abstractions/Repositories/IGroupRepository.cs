using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IGroupRepository : IRepositoryBase<Group, string>
{
    void RemoveConnection(Connection connection);
}