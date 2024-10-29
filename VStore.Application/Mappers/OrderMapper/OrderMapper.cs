using AutoMapper;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.OrderMapper;

public class OrderMapper : Profile
{
    public OrderMapper()
    {
        CreateMap<CartDetail, OrderDetailModel>()
            .ForMember(x => x.ProductName, opt => opt.MapFrom(x => x.Product.Name))
            .ForMember(x => x.ProductQuantity, opt => opt.MapFrom(x => x.Product.Quantity))
            .ForMember(x => x.UnitPrice,
                opt => opt.MapFrom(x => x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice))
            .ForMember(x => x.ItemPrice,
                opt => opt.MapFrom(x =>
                    x.Product.SalePrice == 0 ? x.Product.OriginalPrice * x.Quantity : x.Product.SalePrice * x.Quantity))
            .ForMember(x => x.Thumbnail, opt => opt.MapFrom(x => x.Product.Thumbnail));
        CreateMap<OrderDetailModel, OrderDetail>();
    }
}