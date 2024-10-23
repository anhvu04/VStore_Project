using VStore.Application.Abstractions.BCrypt;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.User.Command.ChangePassword;

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.User)));
        }

        var hashedPassword = _passwordHasher.VerifyPassword(request.OldPassword, user.Password);
        if (!hashedPassword)
        {
            return Result.Failure(DomainError.User.WrongPassword);
        }

        var newPassword = _passwordHasher.HashPassword(request.NewPassword);
        user.Password = newPassword;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(false, true, cancellationToken);
        return Result.Success();
    }
}