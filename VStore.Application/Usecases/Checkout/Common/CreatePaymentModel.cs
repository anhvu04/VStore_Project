namespace VStore.Application.Usecases.Checkout.Common;

public class CreatePaymentModel
{
    public long OrderCode { get; set; }
    public int Amount { get; set; }

    public string Description { get; set; }
    public List<ItemData> Items { get; set; }
}

public class ItemData
{
    public ItemData(string name, int quantity, int price)
    {
        Name = name;
        Quantity = quantity;
        Price = price;
    }

    public string Name { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
}