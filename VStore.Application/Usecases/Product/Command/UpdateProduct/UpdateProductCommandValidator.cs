using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.Product.Command.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OriginalPrice).GreaterThan(0);
        RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Gram).GreaterThan(0);
        RuleFor(x => x.Status).GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(2);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.BrandId).GreaterThan(0);
    }
}