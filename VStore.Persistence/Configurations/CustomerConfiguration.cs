using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VStore.Domain.Entities;

namespace VStore.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasOne(c => c.Cart)
            .WithOne(c => c.Customer)
            .HasForeignKey<Cart>(c => c.CustomerId);
    }
}