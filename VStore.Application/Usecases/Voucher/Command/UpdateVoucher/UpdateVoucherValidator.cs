using FluentValidation;

namespace VStore.Application.Usecases.Voucher.Command.UpdateVoucher;

public class UpdateVoucherValidator : AbstractValidator<UpdateVoucherCommand>
{
    public UpdateVoucherValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        RuleFor(x => x.DiscountPercentage).GreaterThan(0).WithMessage("Discount percentage must be greater than 0");
        RuleFor(x => x.MinPriceCondition).GreaterThan(0).WithMessage("Min price condition must be greater than 0");
        RuleFor(x => x.MaxDiscountAmount).GreaterThan(0).WithMessage("Max discount amount must be greater than 0");
        RuleFor(x => x.ExpiryDate).GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiry date must be greater than current date");
    }
}