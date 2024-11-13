using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.BCrypt;
using VStore.Application.Abstractions.EmailService;
using VStore.Application.Abstractions.GhnService;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Abstractions.RabbitMqService.Consumer;
using VStore.Application.Abstractions.RabbitMqService.Producer;
using VStore.Application.Abstractions.VNPayService;
using VStore.Domain.AuthenticationScheme;
using VStore.Infrastructure.BCrypt;
using VStore.Infrastructure.DependencyInjection.Options;
using VStore.Infrastructure.DependencyInjection.Options.EmailSettings;
using VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;
using VStore.Infrastructure.Email;
using VStore.Infrastructure.Ghn;
using VStore.Infrastructure.JwtBearer;
using VStore.Infrastructure.PayOs;
using VStore.Infrastructure.RabbitMQ;
using VStore.Infrastructure.RabbitMQ.EmailService;
using VStore.Infrastructure.VnPayService;

namespace VStore.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtBearerAuthentication(configuration);
        services.AddEmailService(configuration);
        services.AddRabbitMqService(configuration);
        services.AddDependencies();
        services.AddHostedService<HostedService.AppHostedService>();
    }

    private static void AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication()
            .AddJwtBearer(
                AuthenticationScheme.Access,
                o =>
                {
                    o.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration["Jwt:AccessTokenKey"]!)
                            ),
                            ClockSkew = TimeSpan.FromMinutes(0)
                        };
                }
            ).AddJwtBearer(
                AuthenticationScheme.Refresh,
                o =>
                {
                    o.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration["Jwt:RefreshTokenKey"]!)
                            ),
                            ClockSkew = TimeSpan.FromMinutes(0)
                        };
                }
            );
    }

    private static void AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.EmailSection));
        services.AddSingleton<IEmailService, EmailService>();
    }

    private static void AddRabbitMqService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.RabbitMqSection));
        services.Configure<QueueSettings>(configuration.GetSection(QueueSettings.EmailSection));
        services.AddSingleton<RabbitMqService>();
        services.AddSingleton<IEmailConsumerService, EmailConsumerService>();
        services.AddSingleton<IEmailProducerService, EmailProducerService>();
    }

    private static void AddDependencies(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IPayOsService, PayOsService>();
        services.AddSingleton<IVnPayService, VnPayService.VnPayService>();
        services.AddSingleton<VnPayLibrary>();
        services.AddSingleton<PayOsLibrary>();
        services.AddSingleton<IGhnService, GhnService>();
    }
}