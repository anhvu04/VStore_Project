using Serilog;
using VStore.API.DependencyInjection;
using VStore.Infrastructure.SignalR.MessageHub;
using VStore.Infrastructure.SignalR.PresenceHub;

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
        app.MapHub<MessageHub>("/hubs/message");
        app.MapHub<PresenceHub>("hubs/presence");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}