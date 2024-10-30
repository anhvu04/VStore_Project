using Microsoft.EntityFrameworkCore;
using VStore.Domain.Entities;
using VStore.Persistence.Configurations;

namespace VStore.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.ApplySoftDeleteFilter();
    }

    public virtual DbSet<Brand> Brands { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartDetail> CartDetails { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
}