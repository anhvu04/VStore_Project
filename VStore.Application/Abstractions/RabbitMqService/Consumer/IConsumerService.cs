namespace VStore.Application.Abstractions.RabbitMqService.Consumer;

public interface IConsumerService : IDisposable
{
    Task HandleMessage();
}