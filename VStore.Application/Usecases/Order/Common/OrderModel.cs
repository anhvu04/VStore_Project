namespace VStore.Application.Usecases.Order.Common;

public record OrderModel
{
    public Guid Id { get; set; }
    public string OrderStatus { get; set; }
    public List<DetailModel> ProductDetails { get; set; } = [];
    public int TotalAmount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string PaymentMethod { get; set; }
    public string ShippingCode { get; set; }
}

public record DetailModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
    public int ItemPrice { get; set; }
    public string Thumbnail { get; set; }
}