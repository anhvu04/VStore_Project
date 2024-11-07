using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Order.Command.CancelOrderAdmin;

public record CancelOrderAdminCommand(Guid OrderId) : ICommand
{
}