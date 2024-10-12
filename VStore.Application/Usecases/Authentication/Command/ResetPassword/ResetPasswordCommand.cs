using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;

namespace VStore.Application.Usecases.Authentication.Command.ResetPassword;

public record ResetPasswordCommand : ICommand
{
    [JsonIgnore] public string? Token { get; init; }
    public string Password { get; init; }
    public string ConfirmPassword { get; init; }
}