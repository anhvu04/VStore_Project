using AutoMapper;
using VStore.Application.Usecases.Category.Command.CreateCategory;
using VStore.Application.Usecases.Category.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.CategoryMapper;

public class CategoryMapper : Profile
{
    public CategoryMapper()
    {
        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId == 0 ? (int?)null : src.ParentId));
        CreateMap<Category, CategoryModel>();
    }
}