using VStore.Persistence.DependencyInjection.Extensions;

namespace VStore.API.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
    }
}