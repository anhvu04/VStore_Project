using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.Product.Command.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotNullEmpty();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OriginalPrice).GreaterThan(0);
        RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0)
            .LessThan(x => x.OriginalPrice);
        RuleFor(x => x.Gram).GreaterThan(0);
        RuleFor(x => x.Status).Must(x => x is >= 1 and <= 2).WithMessage("Status must be 1 or 2");
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.BrandId).GreaterThan(0);
    }
}