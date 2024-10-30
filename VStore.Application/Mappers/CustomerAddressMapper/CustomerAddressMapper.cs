using AutoMapper;
using VStore.Application.Usecases.CustomerAddress.Command.CreateCustomerAddress;
using VStore.Application.Usecases.CustomerAddress.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.CustomerAddressMapper;

public class CustomerAddressMapper : Profile
{
    public CustomerAddressMapper()
    {
        CreateMap<CreateCustomerAddressCommand, CustomerAddress>()
            .ForMember(x => x.CustomerId,
                opt => opt.MapFrom(src => src.UserId));
        CreateMap<CustomerAddress, CustomerAddressModel>();
    }
}