using Microsoft.Extensions.Hosting;
using VStore.Application.Abstractions.RabbitMqService.Consumer;
using VStore.Application.Abstractions.RabbitMqService.Producer;

namespace VStore.Infrastructure.HostedService;

public class AppHostedService : IHostedService
{
    private readonly IEmailProducerService _emailProducerService;
    private readonly IEmailConsumerService _emailConsumerService;

    public AppHostedService(IEmailProducerService emailProducerService, IEmailConsumerService emailConsumerService)
    {
        _emailProducerService = emailProducerService;
        _emailConsumerService = emailConsumerService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _emailConsumerService.HandleMessage();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}