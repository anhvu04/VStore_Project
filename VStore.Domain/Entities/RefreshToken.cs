using System.ComponentModel.DataAnnotations.Schema;
using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class RefreshToken : EntityBase<Guid>
{
    [ForeignKey(nameof(UserId))] public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public virtual User User { get; set; }
}