using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VStore.Domain.Entities;

namespace VStore.Persistence.Configurations;

public class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
{
    public void Configure(EntityTypeBuilder<Connection> builder)
    {
        builder.HasOne(c => c.Group)
            .WithMany(g => g.Connections)
            .HasForeignKey(c => c.GroupName)
            .OnDelete(DeleteBehavior.Cascade);
    }
}