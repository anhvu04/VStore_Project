using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.GHNExpress.Common;

namespace VStore.Application.Usecases.GHNExpress.Query.GetWard;

public record GetWardQuery(int DistrictId) : IQuery<List<GetWardModel>>
{
}