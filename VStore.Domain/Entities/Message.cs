using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Entities;

namespace VStore.Domain.Entities;

public class Message : EntityDateAndDeleteBase<Guid>
{
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime? DateRead { get; set; }
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public virtual User Sender { get; set; }
    public virtual User Recipient { get; set; }
}