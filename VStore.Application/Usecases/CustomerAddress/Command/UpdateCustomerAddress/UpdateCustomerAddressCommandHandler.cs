using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.CustomerAddress.Command.UpdateCustomerAddress;

public class UpdateCustomerAddressCommandHandler : ICommandHandler<UpdateCustomerAddressCommand>
{
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerAddressCommandHandler(IMapper mapper, ICustomerAddressRepository customerAddressRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _customerAddressRepository = customerAddressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var customerAddresses = await _customerAddressRepository
            .FindAll(x => x.CustomerId == request.UserId)
            .ToListAsync(cancellationToken: cancellationToken);
        var customerAddress = customerAddresses.FirstOrDefault(x => x.Id == request.Id);
        if (customerAddresses.Count == 0 || customerAddress == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.CustomerAddress)));
        }

        // Automatically set the address as default if it's the only one.
        if (customerAddresses.Count == 1)
        {
            request.IsDefault = true;
        }

        // If the request address is default and more than 1 address exists, then set the default address in db to false
        if (request.IsDefault && customerAddresses.Count > 1)
        {
            var defaultAddress = customerAddresses.FirstOrDefault(x => x.IsDefault && x.Id != request.Id);
            if (defaultAddress != null)
            {
                defaultAddress.IsDefault = false;
                _customerAddressRepository.Update(defaultAddress);
            }
        }

        customerAddress.ReceiverName = request.ReceiverName ?? customerAddress.ReceiverName;
        customerAddress.PhoneNumber = request.PhoneNumber ?? customerAddress.PhoneNumber;
        customerAddress.Address = request.Address ?? customerAddress.Address;
        customerAddress.ProvinceId = request.ProvinceId ?? customerAddress.ProvinceId;
        customerAddress.ProvinceName = request.ProvinceName ?? customerAddress.ProvinceName;
        customerAddress.DistrictId = request.DistrictId ?? customerAddress.DistrictId;
        customerAddress.DistrictName = request.DistrictName ?? customerAddress.DistrictName;
        customerAddress.WardCode = request.WardCode ?? customerAddress.WardCode;
        customerAddress.WardName = request.WardName ?? customerAddress.WardName;
        customerAddress.IsDefault = request.IsDefault;
        _customerAddressRepository.Update(customerAddress);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}