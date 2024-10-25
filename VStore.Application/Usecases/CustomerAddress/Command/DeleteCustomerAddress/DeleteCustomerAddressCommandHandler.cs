using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.CustomerAddress.Command.DeleteCustomerAddress;

public class DeleteCustomerAddressCommandHandler : ICommandHandler<DeleteCustomerAddressCommand>
{
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerAddressCommandHandler(ICustomerAddressRepository customerAddressRepository,
        IUnitOfWork unitOfWork)
    {
        _customerAddressRepository = customerAddressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var customerAddresses = await _customerAddressRepository
            .FindAll(x => x.CustomerId == request.UserId)
            .ToListAsync(cancellationToken: cancellationToken);
        var customerAddress = customerAddresses.FirstOrDefault(x => x.Id == request.AddressId);
        if (customerAddresses.Count == 0 || customerAddress == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.CustomerAddress)));
        }

        /* If the request address is default and more than 1 address exists,
        user must set another address as default first before deleting the default address */
        if (customerAddress.IsDefault && customerAddresses.Count > 1)
        {
            return Result.Failure(DomainError.CustomerAddress.DeleteDefaultAddress);
        }

        _customerAddressRepository.Remove(customerAddress);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}