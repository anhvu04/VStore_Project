using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class Cart : EntityBase<Guid>
{
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<CartDetail> CartDetails { get; set; } = [];
}