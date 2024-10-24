namespace VStore.Application.Usecases.Cart.Common;

public record CartDetailModel
{
    public Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string BrandName { get; init; }
    public required string CategoryName { get; init; }
    public int UnitPrice { get; init; }
    public int Quantity { get; init; }
    public int TotalPrice { get; init; }
}