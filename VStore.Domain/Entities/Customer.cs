using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class Customer
{
    [Key] [ForeignKey(nameof(UserId))] public Guid UserId { get; set; }
    [Required] [Range(10, 10)] public int PhoneNumber { get; set; }
    [Required] [EmailAddress] public string Email { get; set; } = null!;
    [Column(TypeName = "nvarchar(255)")] public string? GoogleId { get; set; }
    [Column(TypeName = "nvarchar(255)")] public string? Address { get; set; }
    public DateOnly? BirthDate { get; set; }
    [Column(TypeName = "varchar(255)")] public string? ProfilePictureUrl { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = [];
    public virtual Cart? Cart { get; set; }
}