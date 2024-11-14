using Microsoft.Extensions.Logging;
using Quartz;
using VStore.Application.Abstractions.RabbitMqService.Consumer;
using VStore.Application.Abstractions.RabbitMqService.Producer;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;

namespace VStore.Infrastructure.Quartz.PayOsService;

[DisallowConcurrentExecution]
public class PayOsPaymentStatus : IJob
{
    private readonly ILogger<PayOsPaymentStatus> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IPayOsProducerService _payOsProducerService;

    public PayOsPaymentStatus(ILogger<PayOsPaymentStatus> logger, IOrderRepository orderRepository,
        IPayOsProducerService payOsProducerService)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _payOsProducerService = payOsProducerService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var orders = _orderRepository.FindAll(x =>
            x.Status == OrderStatus.Pending && x.PaymentMethod == PaymentMethod.Payos).Select(x => x.TransactionCode);
        if (!orders.Any())
        {
            _logger.LogInformation("No available order found");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Send message: Check payos payment status");
        _payOsProducerService.SendMessage(orders);
        return Task.CompletedTask;
    }
}