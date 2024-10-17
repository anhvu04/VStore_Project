using System.Reflection;
using FluentValidation;

namespace VStore.Application.Validators;

public static class NotNullEmptyValidation
{
    public static IRuleBuilderOptions<T, string?> NotNullEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .NotNull();
    }
}