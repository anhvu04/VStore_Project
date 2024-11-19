using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class Connection : EntityBase<string>
{
    public Guid UserId { get; set; }
    public string GroupName { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
}