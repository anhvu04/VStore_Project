using VStore.Domain.Abstractions;
using VStore.Domain.Enums;

namespace VStore.Domain.Entities;

public class OrderLog : EntityBase<Guid>
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public virtual Order Order { get; set; }
}