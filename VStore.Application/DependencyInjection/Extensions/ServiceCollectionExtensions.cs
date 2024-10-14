using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VStore.Application.Behaviors;

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
    }
}