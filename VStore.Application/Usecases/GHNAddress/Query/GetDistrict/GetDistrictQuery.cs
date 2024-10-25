using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.GHNAddress.Common;

namespace VStore.Application.Usecases.GHNAddress.Query.GetDistrict;

public record GetDistrictQuery(int ProvinceId) : IQuery<List<GetDistrictModel>>
{
}