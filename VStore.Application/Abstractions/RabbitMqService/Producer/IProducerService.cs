namespace VStore.Application.Abstractions.RabbitMqService.Producer;

public interface IProducerService : IDisposable
{
    void SendMessage<T>(T message);
}