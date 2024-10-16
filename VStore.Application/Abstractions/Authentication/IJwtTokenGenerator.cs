using System.Security.Claims;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Domain.Shared;

namespace VStore.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    Task<string> GenerateToken(User user, TokenType tokenType);
    Task<DateTime> GetExpirationDate(string token);
    string CreateVerifyCode();
    Result<ClaimsPrincipal> VerifyToken(string token, TokenType tokenType);

    Result VerifyCode(User? user, string token, bool isVerify,
        CancellationToken cancellationToken = default);
    Task<User?> GetUserByToken(string token);
}