using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.Voucher.Command.CreateVoucher;

public class CreateVoucherCommandValidator : AbstractValidator<CreateVoucherCommand>
{
    public CreateVoucherCommandValidator()
    {
        RuleFor(x => x.Code).NotNullEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        RuleFor(x => x.DiscountPercentage).GreaterThan(0).WithMessage("Discount percentage must be greater than 0");
        RuleFor(x => x.MaxDiscountAmount).GreaterThan(0).WithMessage("Max discount amount must be greater than 0");
        RuleFor(x => x.ExpiryDate).GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiry date must be greater than current date");
    }
}