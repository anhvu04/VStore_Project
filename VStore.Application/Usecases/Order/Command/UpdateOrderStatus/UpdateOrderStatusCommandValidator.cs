using FluentValidation;

namespace VStore.Application.Usecases.Order.Command.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderStatus).GreaterThanOrEqualTo(1)
            .WithMessage("OrderStatus must in range [1, 4]").LessThanOrEqualTo(4)
            .WithMessage("OrderStatus must in range [1, 4]");
    }
}