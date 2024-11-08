using AutoMapper;
using VStore.Application.Usecases.Cart.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.CartMapper;

public class CartDetailMapper : Profile
{
    public CartDetailMapper()
    {
        CreateMap<CartDetail, CartModel>();
        CreateMap<CartDetail, CartDetailModel>()
            .ForMember(x => x.ProductName, opt => opt.MapFrom(x => x.Product.Name))
            .ForMember(x => x.BrandName, opt => opt.MapFrom(x => x.Product.Brand.Name))
            .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Product.Category.Name))
            .ForMember(x => x.UnitPrice,
                opt => opt.MapFrom(x => x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice))
            .ForMember(x => x.TotalPrice, opt => opt.MapFrom(x => x.Product.SalePrice == 0
                ? x.Product.OriginalPrice * x.Quantity
                : x.Product.SalePrice * x.Quantity));
    }
}