using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using VStore.Application.Abstractions.RabbitMqService.Producer;
using VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;

namespace VStore.Infrastructure.RabbitMQ.PayOsService;

public class PayOsProducerService : IPayOsProducerService
{
    private readonly IChannel _channel;
    private readonly QueueSettings _queueSettings;
    private readonly ILogger<PayOsProducerService> _logger;

    public PayOsProducerService(RabbitMqService rabbitMqService,
        IOptionsMonitor<QueueSettings> options, ILogger<PayOsProducerService> logger)
    {
        _logger = logger;
        _queueSettings = options.Get(QueueSettings.PayOsSection);
        _channel = rabbitMqService.CreateChannel(true, "PayOs").Result;
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