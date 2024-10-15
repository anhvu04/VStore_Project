using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.Category.Command.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).ValidName();
    }
}