using System.ComponentModel.DataAnnotations;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Authentication.Common;

namespace VStore.Application.Usecases.Authentication.Command;

public record LoginCommand : ICommand<LoginResponseModel>
{
    [Required] public string Username { get; init; }
    [Required] public string Password { get; init; }
}