using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VStore.Application.Abstractions.EmailService;
using VStore.Application.Abstractions.RabbitMqService.Consumer;
using VStore.Application.Models.EmailService;
using VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;

namespace VStore.Infrastructure.RabbitMQ.EmailService;

public class EmailConsumerService : IEmailConsumerService
{
    private readonly ILogger<EmailConsumerService> _logger;
    private readonly IEmailService _emailService;
    private readonly QueueSettings _queueSettings;
    private readonly IChannel _channel;

    public EmailConsumerService(ILogger<EmailConsumerService> logger, IEmailService emailService,
        IOptionsMonitor<QueueSettings> options, RabbitMqService rabbitMqService)
    {
        _logger = logger;
        _emailService = emailService;
        _queueSettings = options.CurrentValue;
        _channel = rabbitMqService.CreateChannel(false).Result;
        _channel.ExchangeDeclareAsync(_queueSettings.ExchangeName, ExchangeType.Direct);
        _channel.QueueDeclareAsync(_queueSettings.QueueName, true, false, false);
        _channel.QueueBindAsync(_queueSettings.QueueName, _queueSettings.ExchangeName, _queueSettings.RoutingKey);
    }


    public async Task HandleMessage()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("Received message: {json}", json);
                var model = System.Text.Json.JsonSerializer.Deserialize<SendMailModel>(json);
                if (model == null)
                {
                    _logger.LogError("Email model is null");
                    return;
                }

                await _emailService.SendEmailAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming email");
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };
        await _channel.BasicConsumeAsync(queue: _queueSettings.QueueName, autoAck: false, consumer: consumer);
    }

    public void Dispose()
    {
        if (_channel.IsOpen)
            _channel.CloseAsync();
        _channel.Dispose();
    }
}