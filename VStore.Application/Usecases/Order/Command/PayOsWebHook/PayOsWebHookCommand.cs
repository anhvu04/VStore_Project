using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Order.Command.PayOsWebHook;

public record PayOsWebHookCommand : ICommand
{
    public required string Codee { get; set; }
    public required string Desc { get; set; }
    public required bool Success { get; set; }
    public required object Data { get; set; }
    public required string Signature { get; set; }
}