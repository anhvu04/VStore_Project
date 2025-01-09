using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Entities;

namespace VStore.Domain.Entities;

public class Voucher : EntityDateBase<Guid>
{
    public required string Code { get; set; }
    public int Quantity { get; set; }
    public int DiscountPercentage { get; set; }
    public int MaxDiscountAmount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = [];
}