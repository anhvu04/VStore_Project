using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.CustomerAddress.Common;
using VStore.Domain.Abstractions;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.CustomerAddress.Query.GetCustomerAddresses;

public class GetCustomerAddressesQueryHandler : IQueryHandler<GetCustomerAddressesQuery, List<CustomerAddressModel>>
{
    private readonly ICustomerAddressRepository _customerAddressRepository;
    private readonly IMapper _mapper;

    public GetCustomerAddressesQueryHandler(ICustomerAddressRepository customerAddressRepository, IMapper mapper)
    {
        _customerAddressRepository = customerAddressRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<CustomerAddressModel>>> Handle(GetCustomerAddressesQuery request,
        CancellationToken cancellationToken)
    {
        var customerAddresses = await _customerAddressRepository.FindAll(x => x.CustomerId == request.UserId)
            .ProjectTo<CustomerAddressModel>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);
        if (!customerAddresses.Any())
        {
            return Result<List<CustomerAddressModel>>.Failure(
                DomainError.CommonError.NotFound(nameof(CustomerAddress)));
        }

        return Result<List<CustomerAddressModel>>.Success(customerAddresses);
    }
}