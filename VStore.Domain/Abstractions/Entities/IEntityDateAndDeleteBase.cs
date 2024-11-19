namespace VStore.Domain.Abstractions.Entities;

public interface IEntityDateAndDeleteBase<TKey> : IEntityBase<TKey>, IDateAndDeleteTracking
{
}