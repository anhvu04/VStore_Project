using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.CustomerAddress.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.CustomerAddress.Query.GetCustomerAddress;

public class GetCustomerAddressQueryHandler : IQueryHandler<GetCustomerAddressQuery, CustomerAddressModel>
{
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly IMapper _mapper;

    public GetCustomerAddressQueryHandler(ICustomerAddressRepository customerAddressRepository, IMapper mapper)
    {
        _customerAddressRepository = customerAddressRepository;
        _mapper = mapper;
    }

    public async Task<Result<CustomerAddressModel>> Handle(GetCustomerAddressQuery request,
        CancellationToken cancellationToken)
    {
        var customerAddress = await _customerAddressRepository
            .FindAll(x => x.Id == request.AddressId && x.CustomerId == request.UserId)
            .ProjectTo<CustomerAddressModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        if (customerAddress == null)
        {
            return Result<CustomerAddressModel>.Failure(DomainError.CommonError.NotFound(nameof(CustomerAddress)));
        }

        return Result<CustomerAddressModel>.Success(customerAddress);
    }
}