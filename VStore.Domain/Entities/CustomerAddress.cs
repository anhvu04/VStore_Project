using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class CustomerAddress : EntityBase<Guid>
{
    [Column(TypeName = "nvarchar(255)")] public string Address { get; set; } = null!;

    public int ProvinceId { get; set; }

    [Column(TypeName = "nvarchar(255)")] public string ProvinceName { get; set; } = null!;

    public int DistrictId { get; set; }

    [Column(TypeName = "nvarchar(255)")] public string DistrictName { get; set; } = null!;

    [Column(TypeName = "nvarchar(255)")] public string WardCode { get; set; } = null!;

    [Column(TypeName = "nvarchar(255)")] public string WardName { get; set; } = null!;

    [Column(TypeName = "nvarchar(255)")]
    [Required]
    public string ReceiverName { get; set; } = null!;

    [Required] public string PhoneNumber { get; set; } = null!;

    public bool IsDefault { get; set; }
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
}