using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VStore.Application.Abstractions.Authentication;
using VStore.Domain.Entities;
using VStore.Domain.Enums;

namespace VStore.Infrastructure.JwtBearer;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private const string Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GenerateToken(User user, TokenType tokenType)
    {
        var claims = await GetClaims(user, tokenType);
        claims.Add(new Claim("TokenType", tokenType.ToString()));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetKey(tokenType)));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetExpiry(tokenType)),
            signingCredentials: cred);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<DateTime> GetExpirationDate(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        return Task.FromResult(jsonToken?.ValidTo ?? DateTime.MinValue);
    }

    public string CreateVerifyCode()
    {
        var code = new StringBuilder();
        var random = new Random();
        for (var i = 0; i < 6; i++)
        {
            code.Append(Code[random.Next(Code.Length)]);
        }

        return code.ToString();
    }

    private int GetExpiry(TokenType tokenType) => tokenType switch
    {
        TokenType.Access => int.Parse(_configuration["Jwt:AccessTokenLifeTime"]!),
        TokenType.Refresh => int.Parse(_configuration["Jwt:RefreshTokenLifeTime"]!),
        TokenType.Verification => int.Parse(_configuration["Jwt:ActivationTokenLifeTime"]!),
        TokenType.ResetPassword => int.Parse(_configuration["Jwt:ActivationTokenLifeTime"]!),
        _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
    };


    private string GetKey(TokenType tokenType) => tokenType switch
    {
        TokenType.Access => _configuration["Jwt:AccessTokenKey"]!,
        TokenType.Refresh => _configuration["Jwt:RefreshTokenKey"]!,
        TokenType.Verification => _configuration["Jwt:ActivationTokenKey"]!,
        TokenType.ResetPassword => _configuration["Jwt:ActivationTokenKey"]!,
        _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
    };


    private Task<List<Claim>> GetClaims(User user, TokenType tokenType)
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
        };
        switch (tokenType)
        {
            case TokenType.Access:
                claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
                break;
            case TokenType.Refresh:
                break;
            case TokenType.Verification:
                claims.Add(new Claim("Token", user.VerificationCode!));
                break;
            case TokenType.ResetPassword:
                claims.Add(new Claim("Token", user.ResetPasswordCode!));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null);
        }

        return Task.FromResult(claims);
    }
}