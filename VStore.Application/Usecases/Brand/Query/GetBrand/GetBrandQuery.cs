using System.Text.Json.Serialization;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Brand.Common;

namespace VStore.Application.Usecases.Brand.Query.GetBrand;

public record GetBrandQuery(int Id) : IQuery<BrandModel>
{
}