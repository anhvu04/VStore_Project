using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VStore.Application.Abstractions.CartService;
using VStore.Application.Behaviors;
using VStore.Application.Services;
using VStore.Domain.Abstractions.Repositories;

namespace VStore.Application.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(AssemblyReference.Assembly);
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(AssemblyReference.Assembly);
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        services.AddDependencyInjection();
    }

    private static void AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<ICartService, CartService>();
    }
}