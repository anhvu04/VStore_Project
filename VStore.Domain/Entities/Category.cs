using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class Category : EntityAuditBase<int>
{
    [Required]
    [Column(TypeName = "nvarchar(255)")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "nvarchar(max)")] public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Product> Products { get; set; } = [];
}