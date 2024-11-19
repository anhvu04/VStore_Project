using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Persistence.Repositories;

namespace VStore.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServerPersistence(configuration);
        services.AddDependencies();
        // Add HttpContextAccessor
        services.AddHttpContextAccessor();
    }

    private static void AddSqlServerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<DbContext, ApplicationDbContext>(builder =>
        {
            // Azure Connection String
            // var connectionString = configuration["AzureConnectionStrings:DefaultConnection"];
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.EnableDetailedErrors()
                .UseLazyLoadingProxies()
                .UseSqlServer(connectionString);
        });
    }

    private static void AddDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartDetailRepository, CartDetailRepository>();
        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}