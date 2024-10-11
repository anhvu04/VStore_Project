using System.Security.Claims;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Authentication.Command.Verify;

public class VerifyCommandHandler : ICommandHandler<VerifyCommand>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(VerifyCommand request, CancellationToken cancellationToken)
    {
        var isVerify = _jwtTokenGenerator.VerifyToken(request.Token, true);
        if (!isVerify.IsSuccess)
        {
            return isVerify;
        }

        var userId = Guid.Parse(isVerify.Value?.Claims.First(x => x.Type == "UserId").Value ?? string.Empty);
        var user = await _userRepository.FindByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(userId)));
        }

        var verifyCode = isVerify.Value?.Claims.First(x => x.Type == "Token").Value;
        if (verifyCode != user.VerificationCode)
        {
            return Result.Failure(DomainError.Authentication.InvalidCode);
        }

        user.IsActive = true;
        user.VerificationCode = null;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(false, true, cancellationToken);
        return Result.Success();
    }
}