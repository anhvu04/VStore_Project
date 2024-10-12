using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Authentication.Command.ForgotPassword;

public record ForgotPasswordCommand : ICommand
{
    public string Email { get; init; }
}