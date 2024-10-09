namespace VStore.Domain.Abstractions.Entities;

public interface IEntityBase<TKey>
{
    public TKey Id { get; set; }
}