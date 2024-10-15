using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.Brand.Command.CreateBrand;

public class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name).ValidName();
    }
}