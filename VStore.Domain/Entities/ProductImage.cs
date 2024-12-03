using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class ProductImage : EntityBase<int>
{
    public string ImageUrl { get; set; }
    public Guid ProductId { get; set; }
    public string PublicId { get; set; }
    public bool IsActive { get; set; }
    public virtual Product Product { get; set; }
}