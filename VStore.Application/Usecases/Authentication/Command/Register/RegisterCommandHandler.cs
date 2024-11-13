using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.BCrypt;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Abstractions.RabbitMqService.Producer;
using VStore.Application.Models.EmailService;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;
using VStore.Domain.Enums;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Authentication.Command.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly IEmailProducerService _emailProducerService;

    public RegisterCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository, IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher,
        IMapper mapper, IEmailProducerService emailProducerService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _emailProducerService = emailProducerService;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await _customerRepository.FindAll(null, x => x.User)
            .SingleOrDefaultAsync(
                x => x.User.UserName == request.Username || x.Email == request.Email ||
                     x.PhoneNumber == request.PhoneNumber, cancellationToken);

        // check exist username, email, phone number
        if (user != null)
        {
            if (user.User.UserName == request.Username)
            {
                return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(user.User.UserName)));
            }

            if (user.Email == request.Email)
            {
                return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(user.Email)));
            }

            if (user.PhoneNumber == request.PhoneNumber)
            {
                return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(user.PhoneNumber)));
            }
        }

        // create verification code, hashed password
        var verificationCode = _jwtTokenGenerator.CreateVerifyCode();
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        // create user
        var userEntity = _mapper.Map<Domain.Entities.User>(request);
        userEntity.Password = hashedPassword;
        userEntity.VerificationCode = verificationCode;
        _userRepository.Add(userEntity);

        // create customer
        var customer = _mapper.Map<Customer>(request);
        customer.UserId = userEntity.Id;
        _customerRepository.Add(customer);
        await _unitOfWork.SaveChangesAsync(false, true, cancellationToken);

        //send verify email
        var token = await _jwtTokenGenerator.GenerateToken(userEntity, TokenType.Verification);
        _emailProducerService.SendMessage(GetVerifyEmailModel(customer.Email, token));
        return Result.Success();
    }

    private SendMailModel GetVerifyEmailModel(string customerEmail, string token)
    {
        var model = new SendMailModel
        {
            To = customerEmail,
            Subject = "Verify Email",
            Body =
                $"Please click the link below to verify your email address: <a href='https://localhost:5000/verify?token={token}'>Verify Email</a>",
            IsBodyHtml = true
        };
        return model;
    }
}