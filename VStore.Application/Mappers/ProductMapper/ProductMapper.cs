using AutoMapper;
using VStore.Application.Usecases.Product.Command.CreateProduct;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.ProductMapper;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<CreateProductCommand, Product>();
    }
}