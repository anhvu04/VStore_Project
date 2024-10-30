using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VStore.Domain.Entities;

namespace VStore.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.Status).HasConversion<int>();
        builder.Property(o => o.PaymentMethod).HasConversion<int>();
    }
}