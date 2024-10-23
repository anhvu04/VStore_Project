namespace VStore.Application.Usecases.Cart.Common;

public record CartModel
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}