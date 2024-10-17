using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Category.Common;

namespace VStore.Application.Usecases.Category.Query.GetCategory;

public record GetCategoryQuery(int Id) : IQuery<CategoryModel>
{
}