using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.User.Command.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.OldPassword).NotNullEmpty();
        RuleFor(x => x.NewPassword).StrongPassword();
        RuleFor(x => x.ConfirmPassword).Matches(x => x.NewPassword).WithMessage("Confirm password must match password");
    }
}