using AutoMapper;
using VStore.Application.Usecases.Authentication.Command.Register;
using VStore.Application.Usecases.Authentication.Common;
using VStore.Domain.Entities;
using VStore.Domain.Enums;

namespace VStore.Application.Mappers.AuthenticationMapper;

public class AuthenticationMapper : Profile
{
    public AuthenticationMapper()
    {
        #region Login

        CreateMap<User, LoginResponseModel>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.Role, opt => opt.MapFrom(x => x.Role.ToString()));

        #endregion

        #region Register

        CreateMap<RegisterCommand, User>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => Guid.NewGuid()))
            .ForMember(x => x.Sex, opt => opt.MapFrom(x => x.Sex))
            .ForMember(x => x.Role, opt => opt.MapFrom(x => Role.Customer))
            .ForMember(x => x.IsActive, opt => opt.MapFrom(x => false));

        CreateMap<RegisterCommand, Customer>().ForMember(x => x.DateOfBirth,
            opt => opt.MapFrom(x => DateOnly.FromDateTime(x.DateOfBirth)));

        #endregion
    }
}