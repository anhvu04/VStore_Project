using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.BCrypt;
using VStore.Application.Abstractions.EmailService;
using VStore.Application.Abstractions.MediatR;
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
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository, IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
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
        var userEntity = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Password = hashedPassword,
            FirstName = request.FirstName,
            LastName = request.LastName,
            VerificationCode = verificationCode,
            Sex = (Sex)request.Sex,
            Role = Role.Customer,
            IsActive = false
        };
        _userRepository.Add(userEntity);

        // create customer
        var customer = new Customer
        {
            UserId = userEntity.Id,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth),
        };
        _customerRepository.Add(customer);
        await _unitOfWork.SaveChangesAsync(false, true, cancellationToken);

        //send verify email
        var token = await _jwtTokenGenerator.GenerateToken(userEntity, TokenType.Verification);
        await _emailService.SendActivationEmailAsync(request.Email, token, true, cancellationToken);
        return Result.Success();
    }
}