using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VStore.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServerPersistence(configuration);
    }

    private static void AddSqlServerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<DbContext, ApplicationDbContext>(builder =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.EnableDetailedErrors()
                .UseLazyLoadingProxies()
                .UseSqlServer(connectionString);
        });
    }
}