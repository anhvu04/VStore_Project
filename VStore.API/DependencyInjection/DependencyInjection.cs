using Microsoft.OpenApi.Models;
using VStore.API.Common;
using VStore.Application.DependencyInjection.Extensions;
using VStore.Infrastructure.DependencyInjection.Extensions;
using VStore.Persistence.DependencyInjection.Extensions;

namespace VStore.API.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddSwagger();
        services.AddFilter();
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc(
                "v1",
                new OpenApiInfo { Title = "VStore", Version = "v1" }
            );
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    private static void AddFilter(this IServiceCollection services)
    {
        services.AddScoped<UserExistFilter>();
    }
}