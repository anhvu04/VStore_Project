using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.Authentication;
using VStore.Application.Abstractions.BCrypt;
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

    public RegisterCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository, IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await _customerRepository.FindAll(null, x => x.User)
            .SingleOrDefaultAsync(
                x => x.User.UserName == request.Username || x.Email == request.Email ||
                     x.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (user != null)
        {
            if (user.User.UserName == request.Username)
            {
                return Result.Failure(DomainError.Authentication.UsernameAlreadyExists);
            }

            if (user.Email == request.Email)
            {
                return Result.Failure(DomainError.Authentication.EmailAlreadyExists);
            }

            if (user.PhoneNumber == request.PhoneNumber)
            {
                return Result.Failure(DomainError.Authentication.PhoneNumberAlreadyExists);
            }
        }

        var verificationCode = _jwtTokenGenerator.CreateVerifyCode();
        var hashedPassword = _passwordHasher.HashPassword(request.Password);
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
        };
        _userRepository.Add(userEntity);
        var customer = new Customer
        {
            UserId = userEntity.Id,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth),
        };
        _customerRepository.Add(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}