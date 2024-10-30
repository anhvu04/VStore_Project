using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Order.Command.PayOsWebHook;

public record PayOsWebHookCommand(string Data) : ICommand
{
}