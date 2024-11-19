using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;

namespace VStore.Infrastructure.RabbitMQ;

public class RabbitMqService : IDisposable
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqService> _logger;
    private readonly RabbitMqSettings _settings;

    public RabbitMqService(ILogger<RabbitMqService> logger,
        IOptionsMonitor<RabbitMqSettings> options)
    {
        _logger = logger;
        _settings = options.CurrentValue;
        if (_connection is not { IsOpen: true })
        {
            _connection = CreateConnection().Result;
        }
    }

    private async Task<IConnection> CreateConnection()
    {
        _logger.LogInformation("Starting RabbitMQ Connection");
        try
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                AutomaticRecoveryEnabled = true
            };
            return await connectionFactory.CreateConnectionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating RabbitMQ Connection");
            throw;
        }
    }

    public IConnection GetConnection()
    {
        if (_connection is not { IsOpen: true })
        {
            _logger.LogInformation("RabbitMQ Connection is not open");
        }

        return _connection;
    }

    private void CloseConnection()
    {
        if (_connection.IsOpen)
        {
            _logger.LogInformation("Closing RabbitMQ Connection");
            _connection.CloseAsync();
        }
    }

    public async Task<IChannel> CreateChannel(bool isProducer, string type)
    {
        try
        {
            _logger.LogInformation("Creating RabbitMQ Channel" +
                                   (isProducer ? " for Producer " + type : " for Consumer " + type));
            return await _connection.CreateChannelAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating RabbitMQ Channel");
            throw;
        }
    }

    public void Dispose()
    {
        CloseConnection();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}