using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.EmailService;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.RabbitMqService.Producer;
using VStore.Application.Models.EmailService;
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
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailProducerService _emailProducerService;

    public ForgotPasswordCommandHandler(ICustomerRepository customerRepository, IEmailService emailService,
        IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IUnitOfWork unitOfWork,
            IEmailProducerService emailProducerService)
    {
        _customerRepository = customerRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _emailProducerService = emailProducerService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var customer =
            await _customerRepository.FindAll(x => x.Email == request.Email,
                x => x.User).FirstOrDefaultAsync(cancellationToken: cancellationToken);
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
        _emailProducerService.SendMessage(GetForgetPasswordEmailModel(customer.Email, token));
        return Result.Success();
    }

    private SendMailModel GetForgetPasswordEmailModel(string customerEmail, string token)
    {
        var model = new SendMailModel
        {
            To = customerEmail,
            Subject = "Reset Password",
            Body =
                $"Please click the link below to reset your password: <a href='https://localhost:5000/reset-password?token={token}'>Reset Password</a>",
            IsBodyHtml = false
        };
        return model;
    }
}