using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Entities;

namespace VStore.Domain.Entities;

public class OrderDetail : EntityBase<Guid>, IDateTracking
{
    [Range(0, int.MaxValue)] public int Quantity { get; set; }
    [Range(1, int.MaxValue)] public int UnitPrice { get; set; }
    [Range(1, int.MaxValue)] public int ItemPrice { get; set; }
    [Column(TypeName = "nvarchar(255)")] public string ProductName { get; set; } = null!;
    [Column(TypeName = "nvarchar(255)")] public string? Thumbnail { get; set; }
    [ForeignKey(nameof(OrderId))] public Guid OrderId { get; set; }
    [ForeignKey(nameof(ProductId))] public Guid ProductId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}