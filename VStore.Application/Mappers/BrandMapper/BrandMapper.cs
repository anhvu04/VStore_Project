using AutoMapper;
using VStore.Application.Usecases.Brand.Command.CreateBrand;
using VStore.Application.Usecases.Brand.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.BrandMapper;

public class BrandMapper : Profile
{
    public BrandMapper()
    {
        CreateMap<Brand, BrandModel>();
        CreateMap<CreateBrandCommand, Brand>();
    }
}