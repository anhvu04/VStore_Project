using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IMessageRepository : IRepositoryBase<Message, Guid>
{
}