using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.BCrypt;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Authentication.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Authentication.Command.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponseModel>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponseModel>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindSingleAsync(x => x.UserName == request.Username, cancellationToken);
        if (user == null)
        {
            return Result<LoginResponseModel>.Failure(DomainError.Authentication.IncorrectUsernameOrPassword);
        }

        var password = _passwordHasher.VerifyPassword(request.Password, user.Password);
        if (!password)
        {
            return Result<LoginResponseModel>.Failure(DomainError.Authentication.IncorrectUsernameOrPassword);
        }

        var token = await _jwtTokenGenerator.GenerateToken(user, TokenType.Access);
        var refreshToken = await _jwtTokenGenerator.GenerateToken(user, TokenType.Refresh);
        var exp = await _jwtTokenGenerator.GetExpirationDate(refreshToken);

        // add refresh token to db
        var nRefreshToken = new Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            Expires = exp,
            UserId = user.Id
        };
        _refreshTokenRepository.Add(nRefreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponseModel
        {
            UserId = user.Id,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            Role = user.Role.ToString(),
            AccessToken = token,
            RefreshToken = refreshToken
        };
        return Result<LoginResponseModel>.Success(response);
    }
}