using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using VStore.Application.Abstractions.RabbitMqService.Producer;
using VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;

namespace VStore.Infrastructure.RabbitMQ.EmailService;

public class EmailProducerService : IEmailProducerService
{
    private readonly QueueSettings _queueSettings;
    private readonly IChannel _channel;
    private readonly ILogger<EmailProducerService> _logger;

    public EmailProducerService(RabbitMqService rabbitMqService,
        ILogger<EmailProducerService> logger, IOptionsMonitor<QueueSettings> options)
    {
        _queueSettings = options.Get(QueueSettings.EmailSection);
        _channel = rabbitMqService.CreateChannel(true, "Email").Result;
        _logger = logger;
    }

    public void SendMessage<T>(T message)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(message);
        var body = System.Text.Encoding.UTF8.GetBytes(json);
        if (_channel.IsClosed)
        {
            _logger.LogError($"Message: {body} failed to send to RabbitMQ. Channel is closed");
            return;
        }

        _logger.LogInformation($"Send message: {json} sent to RabbitMQ");
        _channel.BasicPublishAsync(_queueSettings.ExchangeName, _queueSettings.RoutingKey, body);
    }

    public void Dispose()
    {
        if (_channel.IsOpen)
            _channel.CloseAsync();
        _channel.Dispose();
    }
}