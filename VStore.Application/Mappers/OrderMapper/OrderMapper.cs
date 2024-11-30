using AutoMapper;
using VStore.Application.Usecases.Checkout.Common;
using VStore.Application.Usecases.Order.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.OrderMapper;

public class OrderMapper : Profile
{
    public OrderMapper()
    {
        CreateMap<CartDetail, OrderDetailCheckoutModel>()
            .ForMember(x => x.ProductName, opt => opt.MapFrom(x => x.Product.Name))
            .ForMember(x => x.ProductQuantity, opt => opt.MapFrom(x => x.Product.Quantity))
            .ForMember(x => x.UnitPrice,
                opt => opt.MapFrom(x => x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice))
            .ForMember(x => x.ItemPrice,
                opt => opt.MapFrom(x =>
                    x.Product.SalePrice == 0 ? x.Product.OriginalPrice * x.Quantity : x.Product.SalePrice * x.Quantity))
            .ForMember(x => x.Thumbnail, opt => opt.MapFrom(x => x.Product.Thumbnail));
        CreateMap<OrderDetailCheckoutModel, OrderDetail>();
        CreateMap<Order, OrderModel>()
            .ForMember(x => x.OrderStatus, opt => opt.MapFrom(x => x.Status))
            .ForMember(x => x.ProductDetails, opt => opt.MapFrom(x => x.OrderDetails));
        CreateMap<Order, OrderDetailModel>()
            .ForMember(x => x.OrderStatus, opt => opt.MapFrom(x => x.Status))
            .ForMember(x => x.ProductDetails, opt => opt.MapFrom(x => x.OrderDetails));
        CreateMap<OrderDetail, DetailModel>();
        CreateMap<OrderLog, OrderLogModel>().ForMember(x => x.Status, opt => opt.MapFrom(x => x.Status));
    }
}