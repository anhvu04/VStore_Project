using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.Authentication.Command.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Password)
            .StrongPassword();

        RuleFor(x => x.ConfirmPassword)
            .StrongPassword()
            .Equal(x => x.Password)
            .WithMessage("Confirm password must match password");
    }
}