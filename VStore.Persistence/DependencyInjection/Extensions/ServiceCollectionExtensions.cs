using Microsoft.AspNetCore.Builder;
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
            var env = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
            string connectionString = "";
            if (env == "Development")
            {
                connectionString = configuration.GetConnectionString("DefaultConnection")!;
            }
            else
            {
                var server = configuration["SERVER"] ?? "PANHVU04\\SQLEXPRESS";
                var port = configuration["PORT"] ?? "1433";
                var database = configuration["DATABASE"] ?? "VStoreDB";
                var user = configuration["USER"] ?? "sa";
                var password = configuration["PASSWORD"] ?? "12345";
                connectionString =
                    $"Server={server},{port};Database={database};Persist Security Info=True;User ID={user};Password={password};Pooling=False;Multiple Active Result Sets=True;Encrypt=True;Trust Server Certificate=True";
            }

            Console.WriteLine($"Connection String: {connectionString}");
            // var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.EnableDetailedErrors()
                .UseLazyLoadingProxies()
                .UseSqlServer(connectionString, opt => { opt.CommandTimeout(120); });
        });
    }

    /// <summary>
    /// Auto migrate database (create if not exist, update if exist)
    /// </summary>
    /// <param name="app"></param>
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        dbContext.Database.Migrate();
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
        services.AddScoped<IOrderLogRepository, OrderLogRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}