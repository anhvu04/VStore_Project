using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;
using VStore.Domain.Enums;

namespace VStore.Domain.Entities;

public class Product : EntityAuditBase<Guid>
{
    [Required]
    [Column(TypeName = "nvarchar(255)")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "nvarchar(max)")] public string? Description { get; set; }
    [Range(0, int.MaxValue)] public int Quantity { get; set; }
    [Range(1, int.MaxValue)] public int OriginalPrice { get; set; }
    [Range(0, int.MaxValue)] public int SalePrice { get; set; }
    [Range(1, int.MaxValue)] public int Gram { get; set; }
    [Column(TypeName = "varchar(255)")] public string? Thumbnail { get; set; }
    public bool IsActive { get; set; } = true;
    public ProductStatus Status { get; set; }
    [ForeignKey(nameof(CategoryId))] public int CategoryId { get; set; }
    [ForeignKey(nameof(BrandId))] public int BrandId { get; set; }
    public virtual ICollection<CartDetail> CartDetails { get; set; } = [];
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];
    public virtual Category Category { get; set; } = null!;
    public virtual Brand Brand { get; set; } = null!;
}