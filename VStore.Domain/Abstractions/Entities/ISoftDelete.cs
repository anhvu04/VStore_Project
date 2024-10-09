namespace VStore.Domain.Abstractions.Entities;

public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
}