using Microsoft.Extensions.Hosting;
using VStore.Application.Abstractions.QuartzService;
using VStore.Application.Abstractions.RabbitMqService.Consumer;
using VStore.Application.Abstractions.RabbitMqService.Producer;

namespace VStore.Infrastructure.HostedService;

public class AppHostedService : IHostedService
{
    private readonly IEmailProducerService _emailProducerService;
    private readonly IEmailConsumerService _emailConsumerService;
    private readonly IPayOsProducerService _payOsProducerService;
    private readonly IPayOsConsumerService _payOsConsumerService;
    private readonly IQuartzService _quartzService;

    public AppHostedService(IEmailProducerService emailProducerService, IEmailConsumerService emailConsumerService,
        IQuartzService quartzService, IPayOsProducerService payOsProducerService,
        IPayOsConsumerService payOsConsumerService)
    {
        _emailProducerService = emailProducerService;
        _emailConsumerService = emailConsumerService;
        _quartzService = quartzService;
        _payOsProducerService = payOsProducerService;
        _payOsConsumerService = payOsConsumerService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // handle message from RabbitMQ
        await _emailConsumerService.HandleMessage();
        await _payOsConsumerService.HandleMessage();
        
        // schedule job
        await _quartzService.CheckPayOsPaymentStatusJobAsync(cancellationToken);
        await _quartzService.StartSchedulerAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}