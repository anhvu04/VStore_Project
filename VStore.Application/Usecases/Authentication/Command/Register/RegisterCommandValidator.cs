using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.Authentication.Command.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
        RuleFor(x => x.Password).StrongPassword();
        RuleFor(x => x.ConfirmPassword).Matches(x => x.Password).WithMessage("Confirm password must match password");
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Must be email format");
        RuleFor(x => x.PhoneNumber).PhoneNumber();
        RuleFor(x => x.DateOfBirth).LessThan(DateTime.UtcNow)
            .WithMessage("Date of Birth must be in the past");
        RuleFor(x => x.Sex).Must(x => x is >= 1 and <= 3).WithMessage("Sex is required and in range 1-3");
    }
}