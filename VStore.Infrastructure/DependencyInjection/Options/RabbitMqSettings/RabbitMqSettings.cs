namespace VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;

public class RabbitMqSettings
{
    public const string RabbitMqSection = "RabbitMQSettings";
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}