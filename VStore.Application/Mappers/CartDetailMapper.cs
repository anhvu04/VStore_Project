using AutoMapper;
using VStore.Application.Usecases.Cart.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers;

public class CartDetailMapper : Profile
{
    public CartDetailMapper()
    {
        CreateMap<CartDetail, CartModel>();
    }
}