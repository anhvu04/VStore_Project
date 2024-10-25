using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Authentication.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Authentication.Command.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResponseModel>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefreshTokenResponseModel>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _jwtTokenGenerator.GetUserByToken(request.Token);
        if (user is null)
        {
            return Result<RefreshTokenResponseModel>.Failure(DomainError.CommonError.NotFound(nameof(User)));
        }

        if (user.IsBanned)
        {
            return Result<RefreshTokenResponseModel>.Failure(DomainError.User.Banned);
        }

        var token =
            await _refreshTokenRepository.FindAll(x => x.Token == request.Token && x.UserId == user.Id
            ).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (token is null)
        {
            return Result<RefreshTokenResponseModel>.Failure(
                DomainError.CommonError.NotFound(nameof(RefreshToken)));
        }

        if (token.IsExpired)
        {
            return Result<RefreshTokenResponseModel>.Failure(DomainError.Authentication.TokenExpired);
        }

        // add new access token and remove old refresh token
        var accessToken = await _jwtTokenGenerator.GenerateToken(user, TokenType.Access);
        var refreshToken = await _jwtTokenGenerator.GenerateToken(user, TokenType.Refresh);
        var newToken = new Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = await _jwtTokenGenerator.GetExpirationDate(refreshToken)
        };
        _refreshTokenRepository.Add(newToken);
        _refreshTokenRepository.Remove(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RefreshTokenResponseModel>.Success(new RefreshTokenResponseModel
        {
            Token = accessToken,
            RefreshToken = refreshToken
        });
    }
}