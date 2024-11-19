using VStore.Domain.Abstractions.Entities;

namespace VStore.Domain.Abstractions;

public abstract class EntityDateAndDeleteBase<TKey> : EntityBase<TKey>, IEntityDateAndDeleteBase<TKey>
{
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
}