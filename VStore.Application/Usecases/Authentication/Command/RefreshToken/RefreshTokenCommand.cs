using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Authentication.Common;

namespace VStore.Application.Usecases.Authentication.Command.RefreshToken;

public record RefreshTokenCommand(string Token) : ICommand<RefreshTokenResponseModel>
{
}