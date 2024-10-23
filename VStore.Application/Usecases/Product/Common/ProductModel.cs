using VStore.Domain.Enums;

namespace VStore.Application.Usecases.Product.Common;

public record ProductModel
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int? Quantity { get; init; }
    public int? OriginalPrice { get; init; }
    public int? SalePrice { get; init; }
    public int? Gram { get; init; }
    public string? Thumbnail { get; init; }
    public int? Status { get; init; }
    public int? CategoryId { get; init; }
    public int? BrandId { get; init; }
}