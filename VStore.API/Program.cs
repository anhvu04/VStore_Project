using Microsoft.AspNetCore.StaticFiles;
using Serilog;
using VStore.API.DependencyInjection;
using VStore.Infrastructure.SignalR.MessageHub;
using VStore.Infrastructure.SignalR.PresenceHub;
using VStore.Persistence.DependencyInjection.Extensions;

namespace VStore.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDependencyInjection(builder.Configuration);
        _ = builder.Host.UseSerilog(
            (hostContext, loggerConfiguration) =>
                _ = loggerConfiguration.ReadFrom.Configuration(builder.Configuration));
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        // if (app.Environment.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }

        app.UseSwagger();
        app.UseSwaggerUI();
        app.ApplyMigrations();
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = new FileExtensionContentTypeProvider
            {
                Mappings =
                {
                    [".jpg"] = "image/jpeg",
                    [".jpeg"] = "image/jpeg",
                    [".png"] = "image/png",
                    [".gif"] = "image/gif",
                    [".webp"] = "image/webp",
                    [".svg"] = "image/svg+xml",
                }
            },
            OnPrepareResponse = ctx =>
            {
                // allow all origins (for demo purposes only)
                ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                // Disable caching
                ctx.Context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate");
                ctx.Context.Response.Headers.Append("Pragma", "no-cache");
                ctx.Context.Response.Headers.Append("Expires", "0");
            }
        });
        app.MapHub<MessageHub>("/hubs/message");
        app.MapHub<PresenceHub>("hubs/presence");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}