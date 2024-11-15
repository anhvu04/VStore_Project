using VStore.Domain.Abstractions;

namespace VStore.Domain.Entities;

public class Group : EntityBase<string>
{
    public virtual ICollection<Connection> Connections { get; set; } = [];
}