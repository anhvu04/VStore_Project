using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.BCrypt;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Authentication.Command.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;

    public ResetPasswordCommandHandler(IJwtTokenGenerator tokenGenerator, IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher, IUserRepository userRepository)
    {
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var isVerify = _tokenGenerator.VerifyToken(request.Token!, TokenType.ResetPassword);
        if (!isVerify.IsSuccess)
        {
            return isVerify;
        }

        var userId = Guid.Parse(isVerify.Value?.Claims.First(x => x.Type == "UserId").Value ?? string.Empty);
        var user = await _userRepository.FindByIdAsync(userId, cancellationToken);
        var resetPasswordCode = isVerify.Value?.Claims.First(x => x.Type == "Token").Value ?? string.Empty;
        var isVerifyCode = _tokenGenerator.VerifyCode(user, resetPasswordCode, false, cancellationToken);
        if (!isVerifyCode.IsSuccess)
        {
            return isVerifyCode;
        }

        user!.Password = _passwordHasher.HashPassword(request.Password);
        user.ResetPasswordCode = null;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(false, true, cancellationToken);
        return Result.Success();
    }
}