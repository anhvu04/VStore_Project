using FluentValidation;

namespace VStore.Application.Validators;

public static class StrongPasswordValidation
{
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        const string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$";
        return ruleBuilder
            .Matches(pattern)
            .WithMessage(
                "Password must be 8-30 characters, contain at least one uppercase letter, one lowercase letter, one digit and one special character");
    }
}