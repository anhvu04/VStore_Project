using FluentValidation;

namespace VStore.Application.Usecases.Brand.Command.CreateBrand;

public class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name is required");
    }
}