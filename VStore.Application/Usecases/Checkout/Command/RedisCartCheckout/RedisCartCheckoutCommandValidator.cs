using FluentValidation;

namespace VStore.Application.Usecases.Checkout.Command.RedisCartCheckout;

public class RedisCartCheckoutCommandValidator : AbstractValidator<RedisCartCheckoutCommand>
{
    public RedisCartCheckoutCommandValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();
        RuleFor(x => x.PaymentMethod).Must(x => x is >= 1 and <= 3)
            .WithMessage("Payment method must in range [1, 3]");
        RuleFor(x => x.ShippingFee).Must(x => x >= 0)
            .WithMessage("Shipping fee must be greater than or equal to 0");
    }
}