using FluentValidation;

namespace VStore.Application.Validators;

public static class NameValidation
{
    public static IRuleBuilderOptions<T, string?> ValidName<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .NotNull()
            .WithMessage("Name is required");
    }
}