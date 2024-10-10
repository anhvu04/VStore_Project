using FluentValidation;

namespace VStore.Application.Validators;

public static class PhoneNumberValidation
{
    public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        const string pattern = @"(84|0[3|5|7|8|9])+([0-9]{8})\b";
        return ruleBuilder
            .Must(x => x.Length == 10).WithMessage("Phone number must be 10 digits")
            .Must(x => System.Text.RegularExpressions.Regex.IsMatch(x, pattern))
            .WithMessage("Phone number must be a valid phone number (+84)");
    }
}