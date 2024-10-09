using System.ComponentModel.DataAnnotations;
using VStore.Domain.Abstractions.Entities;

namespace VStore.Domain.Abstractions;

public abstract class EntityBase<TKey> : IEntityBase<TKey>
{
    [Key] public TKey Id { get; set; }
}