using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.EmailService;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Authentication.Command.ForgotPassword;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailService _emailService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(ICustomerRepository customerRepository, IEmailService emailService,
        IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _emailService = emailService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var customer =
            await _customerRepository.FindSingleAsync(x => x.Email == request.Email, cancellationToken, x => x.User);
        if (customer is null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Customer)));
        }

        if (customer.User.IsBanned)
        {
            return Result.Failure(DomainError.User.Banned);
        }

        var code = _jwtTokenGenerator.CreateVerifyCode();
        customer.User.ResetPasswordCode = code;
        _userRepository.Update(customer.User);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var token = await _jwtTokenGenerator.GenerateToken(customer.User, TokenType.ResetPassword);
        await _emailService.SendActivationEmailAsync(customer.Email, token, false, cancellationToken);
        return Result.Success();
    }
}