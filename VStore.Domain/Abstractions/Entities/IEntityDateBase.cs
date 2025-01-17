namespace VStore.Domain.Abstractions.Entities;

public interface IEntityDateBase<TKey> : IEntityBase<TKey>, IDateTracking
{
}