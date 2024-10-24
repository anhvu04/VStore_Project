namespace VStore.Application.Usecases.Product.Common;

public record ProductResponseModel : ProductModel
{
    public new string? Status { get; init; }
    public string? CategoryName { get; init; }
    public string? BrandName { get; init; }
}