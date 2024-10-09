using System.ComponentModel.DataAnnotations;
using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class CartDetail : EntityBase<Guid>
{
    [Range(1, int.MaxValue)] public int Quantity { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public virtual Cart Cart { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}