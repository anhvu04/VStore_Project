using AutoMapper;
using VStore.Application.Usecases.Category.Command.CreateCategory;
using VStore.Application.Usecases.Category.Common;
using VStore.Domain.Entities;

namespace VStore.Application.Mappers.CategoryMapper;

public class CategoryMapper : Profile
{
    public CategoryMapper()
    {
        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<Category, CategoryModel>();
    }
}