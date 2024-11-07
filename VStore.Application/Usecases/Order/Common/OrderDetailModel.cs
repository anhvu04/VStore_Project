namespace VStore.Application.Usecases.Order.Common;

public record OrderDetailModel : OrderModel
{
    public string ReceiverName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public int ShippingFee { get; set; }
}