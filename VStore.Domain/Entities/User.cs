using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Entities;
using VStore.Domain.Enums;

namespace VStore.Domain.Entities;

public class User : EntityBase<Guid>, IDateTracking, ISoftDelete
{
    [Required] [Range(5, 20)] public string UserName { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    [Column(TypeName = "nvarchar(255)")] public string? FirstName { get; set; }
    [Column(TypeName = "nvarchar(255)")] public string? LastName { get; set; }
    [Column(TypeName = "varchar(6)")] public string? VerificationCode { get; set; }
    [Column(TypeName = "varchar(6)")] public string? ResetPasswordCode { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsBanned { get; set; } = false;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Role Role { get; set; }
    public Sex Sex { get; set; }
    public virtual Customer? Customer { get; set; }
}