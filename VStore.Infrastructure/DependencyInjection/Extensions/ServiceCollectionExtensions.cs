using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using StackExchange.Redis;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.BCrypt;
using VStore.Application.Abstractions.CloudinaryService;
using VStore.Application.Abstractions.EmailService;
using VStore.Application.Abstractions.GhnService;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Abstractions.QuartzService;
using VStore.Application.Abstractions.RabbitMqService.Consumer;
using VStore.Application.Abstractions.RabbitMqService.Producer;
using VStore.Application.Abstractions.RedisCartService;
using VStore.Application.Abstractions.VNPayService;
using VStore.Domain.AuthenticationScheme;
using VStore.Infrastructure.BCrypt;
using VStore.Infrastructure.Cloudinary;
using VStore.Infrastructure.DependencyInjection.Options.EmailSettings;
using VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;
using VStore.Infrastructure.Email;
using VStore.Infrastructure.Ghn;
using VStore.Infrastructure.JwtBearer;
using VStore.Infrastructure.PayOs;
using VStore.Infrastructure.Quartz;
using VStore.Infrastructure.RabbitMQ;
using VStore.Infrastructure.RabbitMQ.EmailService;
using VStore.Infrastructure.RabbitMQ.PayOsService;
using VStore.Infrastructure.Redis.RedisCacheResponse;
using VStore.Infrastructure.Redis.RedisCartService;
using VStore.Infrastructure.SignalR.PresenceHub;
using VStore.Infrastructure.VnPay;

namespace VStore.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtBearerAuthentication(configuration);
        services.AddEmailService(configuration);
        services.AddRabbitMqService(configuration);
        services.AddQuartzService(configuration);
        services.AddSignalRService();
        services.AddDependencies();
        services.AddHostedService<HostedService.AppHostedService>();
        services.AddCloudinaryService(configuration);
        services.AddRedisService(configuration);
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
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
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
        services.Configure<QueueSettings>(QueueSettings.EmailSection,
            configuration.GetSection(QueueSettings.EmailSection));
        services.Configure<QueueSettings>(QueueSettings.PayOsSection,
            configuration.GetSection(QueueSettings.PayOsSection));
        services.AddSingleton<RabbitMqService>();
        services.AddSingleton<IEmailConsumerService, EmailConsumerService>();
        services.AddSingleton<IEmailProducerService, EmailProducerService>();
        services.AddSingleton<IPayOsConsumerService, PayOsConsumerService>();
        services.AddSingleton<IPayOsProducerService, PayOsProducerService>();
    }

    private static void AddQuartzService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddQuartz(q => { q.UseMicrosoftDependencyInjectionJobFactory(); });
        services.AddSingleton(
            provider =>
            {
                var scheduler = provider.GetRequiredService<ISchedulerFactory>().GetScheduler().Result;
                // Must have job factory to register job
                scheduler.JobFactory = provider.GetRequiredService<IJobFactory>();
                return scheduler;
            });
        services.AddSingleton<IQuartzService, QuartzService>();
    }

    private static void AddSignalRService(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
    }

    private static void AddCloudinaryService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICloudinaryService, CloudinaryService>();
    }

    private static void AddRedisService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(config =>
        {
            var env = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
            string connectionString = "";
            if (env == "Development")
            {
                connectionString = configuration.GetConnectionString("Redis")!;
            }
            else
            {
                var host = configuration["REDIS_HOST"] ?? "host.docker.internal";
                var password = configuration["REDIS_PASSWORD"];
                if (string.IsNullOrEmpty(password))
                {
                    throw new Exception("Redis password is not set");
                }

                connectionString = $"redis://{host}:6379,password={password}";
            }

            Console.WriteLine($"Redis Connection String: {connectionString}");
            var redisConfig = ConfigurationOptions.Parse(connectionString, true);
            return ConnectionMultiplexer.Connect(redisConfig);
        });
        services.AddSingleton<IRedisCartService, RedisCartService>();
        services.AddSingleton<IRedisCacheResponse, RedisCacheResponse>();
    }

    private static void AddDependencies(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IPayOsService, PayOsService>();
        services.AddSingleton<IVnPayService, VnPayService>();
        services.AddSingleton<VnPayLibrary>();
        services.AddSingleton<PayOsLibrary>();
        services.AddSingleton<IGhnService, GhnService>();
    }
}