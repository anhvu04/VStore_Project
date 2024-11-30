using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VStore.Application.Abstractions.PayOsService;
using VStore.Application.Abstractions.RabbitMqService.Consumer;
using VStore.Application.Models.PayOsService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Infrastructure.DependencyInjection.Options.RabbitMqSettings;
using VStore.Infrastructure.SignalR.PresenceHub;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace VStore.Infrastructure.RabbitMQ.PayOsService;

public class PayOsConsumerService : IPayOsConsumerService
{
    private readonly ILogger<PayOsConsumerService> _logger;
    private readonly IChannel _channel;
    private readonly QueueSettings _queueSettings;
    private readonly IPayOsService _payOsService;
    private readonly IServiceProvider _serviceProvider;
    private readonly PresenceTracker _presenceTracker;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public PayOsConsumerService(ILogger<PayOsConsumerService> logger, RabbitMqService rabbitMqService,
        IOptionsMonitor<QueueSettings> options, IPayOsService payOsService, IServiceProvider serviceProvider,
        PresenceTracker presenceTracker, IHubContext<PresenceHub> presenceHub)
    {
        _logger = logger;
        _payOsService = payOsService;
        _serviceProvider = serviceProvider;
        _presenceTracker = presenceTracker;
        _presenceHub = presenceHub;
        _queueSettings = options.Get(QueueSettings.PayOsSection);
        _channel = rabbitMqService.CreateChannel(false, "PayOs").Result;
        _channel.ExchangeDeclareAsync(_queueSettings.ExchangeName, ExchangeType.Direct);
        _channel.QueueDeclareAsync(_queueSettings.QueueName, true, false, false);
        _channel.QueueBindAsync(_queueSettings.QueueName, _queueSettings.ExchangeName, _queueSettings.RoutingKey);
    }

    public async Task HandleMessage()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var transactionCodes = JsonSerializer.Deserialize<List<int>>(json);
                if (transactionCodes == null || transactionCodes.Count == 0)
                {
                    _logger.LogError("Message is null or empty");
                    return;
                }

                foreach (var orderCode in transactionCodes)
                {
                    var payment = await _payOsService.GetPaymentInformation(orderCode);
                    if (!payment.IsSuccess)
                    {
                        _logger.LogError($"Get payment information for order: {orderCode} failed");
                        continue;
                    }

                    var data = JsonSerializer.Serialize(payment.Value!.Data);

                    var paymentStatus =
                        JsonSerializer.Deserialize<PayOsPaymentStatusResponseModel>(data);
                    if (paymentStatus == null || paymentStatus.Data == null)
                    {
                        _logger.LogError("Payment status is null");
                        continue;
                    }

                    if (paymentStatus.Data.Status != "CANCELLED" && paymentStatus.Data.Status != "EXPIRED")
                    {
                        _logger.LogInformation($"Order: {orderCode} is paid");
                        continue;
                    }

                    var serviceProvider = _serviceProvider.CreateScope().ServiceProvider;
                    var orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();
                    var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
                    var productRepository = serviceProvider.GetRequiredService<IProductRepository>();
                    var order = await orderRepository.FindAll(x => x.TransactionCode == orderCode)
                        .Include(x => x.OrderDetails).ThenInclude(x => x.Product)
                        .FirstOrDefaultAsync();
                    if (order == null)
                    {
                        _logger.LogError($"Order: {orderCode} not found");
                        continue;
                    }

                    if (order.Status == OrderStatus.Cancelled)
                    {
                        _logger.LogInformation($"Order: {orderCode} is already cancelled");
                        continue;
                    }

                    _logger.LogInformation($"Order: {orderCode} is cancelled successfully");
                    order.Status = OrderStatus.Cancelled;
                    order.OrderLogs.Add(new OrderLog
                    {
                        // OrderId = order.Id,
                        Status = OrderStatus.Cancelled,
                        CreatedDate = DateTime.UtcNow
                    });
                    foreach (var product in order.OrderDetails)
                    {
                        if (product.Product.Status == ProductStatus.OutOfStock)
                        {
                            product.Product.Status = ProductStatus.Selling;
                        }

                        product.Product.Quantity += product.Quantity;
                        productRepository.Update(product.Product);
                    }

                    orderRepository.Update(order);
                    await unitOfWork.SaveChangesAsync();

                    // Add SignalR notification
                    var connectionId = await _presenceTracker.GetConnectionsForUser(order.CustomerId);
                    if (connectionId != null)
                    {
                        await _presenceHub.Clients.Clients(connectionId).SendAsync("OrderUpdated", order.Status);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming payos");
            }

            await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        };
        await _channel.BasicConsumeAsync(_queueSettings.QueueName, false, consumer);
    }


    public void Dispose()
    {
        if (_channel.IsOpen)
            _channel.CloseAsync();
        _channel.Dispose();
    }
}