using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.GHNAddress.Common;

namespace VStore.Application.Usecases.GHNAddress.Query.GetWard;

public record GetWardQuery(int DistrictId) : IQuery<List<GetWardModel>>
{
}