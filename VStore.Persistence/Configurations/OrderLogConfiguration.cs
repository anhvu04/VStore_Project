using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VStore.Domain.Entities;
using VStore.Domain.Enums;

namespace VStore.Persistence.Configurations;

public class OrderLogConfiguration : IEntityTypeConfiguration<OrderLog>
{
    public void Configure(EntityTypeBuilder<OrderLog> builder)
    {
        builder.Property(o => o.Status).HasConversion<int>();
    }
}