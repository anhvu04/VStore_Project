namespace VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;

public class QueueSettings
{
    private const string Section = "QueueSettings";
    public const string EmailSection = $"{Section}:Email";
    public string ExchangeName { get; set; }
    public string RoutingKey { get; set; }
    public string QueueName { get; set; }
}