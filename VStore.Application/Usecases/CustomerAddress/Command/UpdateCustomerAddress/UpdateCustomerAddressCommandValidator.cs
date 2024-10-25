using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.CustomerAddress.Command.UpdateCustomerAddress;

public class UpdateCustomerAddressCommandValidator : AbstractValidator<UpdateCustomerAddressCommand>
{
    public UpdateCustomerAddressCommandValidator()
    {
        RuleFor(x => x.PhoneNumber!).PhoneNumber();
        RuleFor(x => x.ProvinceId).GreaterThanOrEqualTo(201).WithMessage("Province Id must in range [201,269]")
            .LessThanOrEqualTo(269).WithMessage("Province Id must in range [201,269]");
        RuleFor(x => x.DistrictId).GreaterThanOrEqualTo(1442).WithMessage("District Id must in range [1442,3696]")
            .LessThanOrEqualTo(3696).WithMessage("District Id must in range [1442,3696]");
    }
}