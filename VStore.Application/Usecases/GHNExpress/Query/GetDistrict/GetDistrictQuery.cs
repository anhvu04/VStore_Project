using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.GHNExpress.Common;

namespace VStore.Application.Usecases.GHNExpress.Query.GetDistrict;

public record GetDistrictQuery(int ProvinceId) : IQuery<List<GetDistrictModel>>
{
}