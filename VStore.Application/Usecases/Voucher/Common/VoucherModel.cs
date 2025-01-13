namespace VStore.Application.Usecases.Voucher.Common;

public record VoucherModel
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public int? Quantity { get; set; }
    public int? DiscountPercentage { get; set; }
    public int? MinPriceCondition { get; set; }
    public int? MaxDiscountAmount { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
}