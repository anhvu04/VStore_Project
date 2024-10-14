namespace VStore.Application.Usecases.Authentication.Common;

public class RefreshTokenResponseModel
{
    public string Token { get; init; }
    public string RefreshToken { get; init; }
}