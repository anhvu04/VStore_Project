using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class Customer : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    [Required] public string PhoneNumber { get; set; }
    [Required] [EmailAddress] public string Email { get; set; } = null!;
    [Column(TypeName = "nvarchar(255)")] public string? GoogleId { get; set; }
    [Column(TypeName = "nvarchar(255)")] public string? Address { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    [Column(TypeName = "varchar(255)")] public string? ProfilePictureUrl { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = [];
    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = [];
    public virtual Cart? Cart { get; set; }
}