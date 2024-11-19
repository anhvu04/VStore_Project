namespace VStore.Application.Models.SignalRService;

public class MessageModel
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime CreatedDate { get; set; }
}