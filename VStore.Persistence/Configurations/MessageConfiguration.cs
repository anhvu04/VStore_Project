using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VStore.Domain.Entities;

namespace VStore.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(x => x.Sender)
            .WithMany(x => x.MessagesSent)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior
                .Restrict); // Prevents cascade delete to avoid deleting users when messages are deleted
        builder.HasOne(x => x.Recipient)
            .WithMany(x => x.MessagesReceived)
            .HasForeignKey(x => x.RecipientId)
            .OnDelete(DeleteBehavior
                .Restrict); // Prevents cascade delete to avoid deleting users when messages are deleted
    }
}