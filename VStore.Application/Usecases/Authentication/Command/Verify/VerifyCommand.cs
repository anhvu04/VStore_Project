using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Authentication.Command.Verify;

public record VerifyCommand : ICommand
{
    public string Token { get; init; }
}