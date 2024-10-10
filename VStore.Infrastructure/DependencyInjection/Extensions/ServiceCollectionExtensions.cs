using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace VStore.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtBearerAuthentication(configuration);
    }

    private static void AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication()
            .AddJwtBearer(
                "Access",
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
}