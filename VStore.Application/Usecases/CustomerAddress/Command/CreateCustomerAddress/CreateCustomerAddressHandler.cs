using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.CustomerAddress.Command.CreateCustomerAddress;

public class CreateCustomerAddressHandler : ICommandHandler<CreateCustomerAddressAddressCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly IMapper _mapper;

    public CreateCustomerAddressHandler(IUnitOfWork unitOfWork, ICustomerAddressRepository customerAddressRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _customerAddressRepository = customerAddressRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(CreateCustomerAddressAddressCommand request, CancellationToken cancellationToken)
    {
        var customerAddress = await _customerAddressRepository
            .FindAll(x => x.CustomerId == request.UserId).ToListAsync(cancellationToken: cancellationToken);
        if (customerAddress.Count == 3)
        {
            return Result.Failure(DomainError.CustomerAddress.LimitAddress);
        }

        if (customerAddress.Count == 0)
        {
            request.IsDefault = true;
        }

        // If the request address is default and more than 1 address exists, then set the default address in db to false
        if (request.IsDefault && customerAddress.Count != 0)
        {
            try
            {
                var addressDefault = customerAddress.SingleOrDefault(x => x.IsDefault);
                if (addressDefault == null)
                {
                    return Result.Failure(DomainError.CustomerAddress.DefaultAddressNotFoundOrMoreThanOne);
                }

                addressDefault.IsDefault = false;
                _customerAddressRepository.Update(addressDefault);
            }
            catch (InvalidOperationException)
            {
                return Result.Failure(DomainError.CustomerAddress.DefaultAddressNotFoundOrMoreThanOne);
            }
        }

        var entity = _mapper.Map<Domain.Entities.CustomerAddress>(request);
        _customerAddressRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}