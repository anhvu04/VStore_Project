using FluentValidation;
using VStore.Application.Validators;

namespace VStore.Application.Usecases.CustomerAddress.Command.CreateCustomerAddress;

public class CreateCustomerAddressCommandValidator : AbstractValidator<CreateCustomerAddressAddressCommand>
{
    public CreateCustomerAddressCommandValidator()
    {
        RuleFor(x => x.ReceiverName).NotEmpty().WithMessage("Receiver name is required");
        RuleFor(x => x.PhoneNumber!).PhoneNumber();
        RuleFor(x => x.Address).NotNullEmpty();
        RuleFor(x => x.ProvinceId).GreaterThanOrEqualTo(201).WithMessage("Province Id must in range [201,269]")
            .LessThanOrEqualTo(269).WithMessage("Province Id must in range [201,269]");
        RuleFor(x => x.ProvinceName).NotNullEmpty();
        RuleFor(x => x.DistrictId).GreaterThanOrEqualTo(1442).WithMessage("District Id must in range [1442,3696]")
            .LessThanOrEqualTo(3696).WithMessage("District Id must in range [1442,3696]");
        RuleFor(x => x.DistrictName).NotNullEmpty();
        RuleFor(x => x.WardCode).NotNullEmpty();
        RuleFor(x => x.WardName).NotNullEmpty();
    }
}