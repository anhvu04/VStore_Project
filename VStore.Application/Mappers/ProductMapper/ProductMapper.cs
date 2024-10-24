using AutoMapper;
using VStore.Application.Usecases.Product.Command.CreateProduct;
using VStore.Application.Usecases.Product.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.ProductMapper;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<Product, ProductResponseModel>()
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.Status.ToString()))
            .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category.Name))
            .ForMember(x => x.BrandName, opt => opt.MapFrom(x => x.Brand.Name));
    }
}