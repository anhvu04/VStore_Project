namespace VStore.Application.Usecases.Cart.Common;

public record CartModel
{
    public object CartDetails { get; set; } = null!;
    public int TotalItems { get; set; }
    public int TotalAmount { get; set; }
}