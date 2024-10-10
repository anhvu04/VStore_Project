using System.ComponentModel.DataAnnotations;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Authentication.Common;

namespace VStore.Application.Usecases.Authentication.Command.Login;

public record LoginCommand : ICommand<LoginResponseModel>
{
    public string Username { get; init; }
    public string Password { get; init; }
}