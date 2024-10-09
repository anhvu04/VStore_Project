using VStore.Domain.Abstractions.Entities;

namespace VStore.Domain.Abstractions;

public abstract class EntityAuditBase<TKey> : EntityBase<TKey>, IEntityAuditBase<TKey>
{
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
}