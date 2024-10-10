using VStore.Domain.Entities;
using VStore.Domain.Enums;

namespace VStore.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    Task<string> GenerateToken(User user, TokenType tokenType);
    Task<DateTime> GetExpirationDate(string token);
    string CreateVerifyCode();
}