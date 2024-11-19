using AutoMapper;
using VStore.Application.Models.SignalRService;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.MessageMapper;

public class MessageMapper : Profile
{
    public MessageMapper()
    {
        CreateMap<Message, MessageModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.RecipientId));
    }
}