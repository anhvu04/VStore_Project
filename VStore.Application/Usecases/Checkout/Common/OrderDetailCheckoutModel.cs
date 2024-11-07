namespace VStore.Application.Usecases.Checkout.Common;

public record OrderDetailCheckoutModel
{
    public Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public int ProductQuantity { get; init; }
    public int Quantity { get; init; }
    public int UnitPrice { get; init; }
    public int ItemPrice { get; init; }
    public string? Thumbnail { get; init; }
}