using AutoMapper;
using VStore.Application.Usecases.CustomerAddress.Command.CreateCustomerAddress;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.CustomerAddressMapper;

public class CustomerAddressMapper : Profile
{
    public CustomerAddressMapper()
    {
        CreateMap<CreateCustomerAddressAddressCommand, CustomerAddress>()
            .ForMember(x => x.CustomerId, 
                opt => opt.MapFrom(src => src.UserId));
    }
}