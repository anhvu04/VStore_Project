using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Entities;
using VStore.Domain.Enums;

namespace VStore.Domain.Entities;

public class Order : EntityBase<Guid>, IDateTracking
{
    [Range(1, int.MaxValue)] public int TotalPrice { get; set; }
    [Range(1, int.MaxValue)] public int ShippingFee { get; set; }
    [Range(1, int.MaxValue)] public int TotalAmount { get; set; }
    [Range(1, int.MaxValue)] public int TotalGram { get; set; }
    [Column(TypeName = "nvarchar(max)")] public string? Note { get; set; }
    [Required] public string ReceiverName { get; set; } = null!;
    [Required] [Range(10, 10)] public int PhoneNumber { get; set; }
    [Required] [EmailAddress] public string Email { get; set; } = null!;
    [Range(0, int.MaxValue)] public int DistrictId { get; set; }
    [Column(TypeName = "nvarchar(255)")] public string WardCode { get; set; } = null!;
    [Column(TypeName = "varchar(255)")] public string? PaymentMethod { get; set; }
    public int? TransactionCode { get; set; }
    [Column(TypeName = "nvarchar(255)")] public string? ShippingCode { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    [ForeignKey(nameof(CustomerId))] public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];
}