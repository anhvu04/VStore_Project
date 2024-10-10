using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.BCrypt;
using VStore.Domain.AuthenticationScheme;
using VStore.Infrastructure.BCryptHash;
using VStore.Infrastructure.JwtBearer;

namespace VStore.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtBearerAuthentication(configuration);
        services.AddDependencies(configuration);
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
            );
        // .AddJwtBearer(
        //     "Refresh",
        //     o =>
        //     {
        //         o.TokenValidationParameters =
        //             new TokenValidationParameters
        //             {
        //                 ValidateIssuer = true,
        //                 ValidateAudience = false,
        //                 ValidateLifetime = true,
        //                 ValidIssuer = configuration["Jwt:Issuer"],
        //                 ValidateIssuerSigningKey = true,
        //                 IssuerSigningKey = new SymmetricSecurityKey(
        //                     Encoding.UTF8.GetBytes(configuration["Jwt:RefreshTokenKey"]!)
        //                 ),
        //                 ClockSkew = TimeSpan.FromMinutes(0)
        //             };
        //     }
        // )
        // .AddJwtBearer(
        //     "Activation",
        //     o =>
        //     {
        //         o.TokenValidationParameters =
        //             new TokenValidationParameters
        //             {
        //                 ValidateIssuer = true,
        //                 ValidateAudience = false,
        //                 ValidateLifetime = true,
        //                 ValidIssuer = configuration["Jwt:Issuer"],
        //                 ValidateIssuerSigningKey = true,
        //                 IssuerSigningKey = new SymmetricSecurityKey(
        //                     Encoding.UTF8.GetBytes(configuration["Jwt:ActivationTokenKey"]!)
        //                 ),
        //                 ClockSkew = TimeSpan.FromMinutes(0)
        //             };
        //     }
        // );
    }

    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
    }
}