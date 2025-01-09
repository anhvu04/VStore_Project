using VStore.Domain.Abstractions.Entities;

namespace VStore.Domain.Abstractions;

public abstract class EntityDateBase<TKey> : EntityBase<TKey>, IEntityDateBase<TKey>
{
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}