using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Application.Usecases.Brand.Common;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Brand.Query.GetBrand;

public class GetBrandQueryHandler : IQueryHandler<GetBrandQuery, BrandModel>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;

    public GetBrandQueryHandler(IBrandRepository brandRepository, IMapper mapper)
    {
        _brandRepository = brandRepository;
        _mapper = mapper;
    }

    public async Task<Result<BrandModel>> Handle(GetBrandQuery request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.FindAll(x => x.Id == request.Id)
            .ProjectTo<BrandModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        if (brand == null)
        {
            return Result<BrandModel>.Failure(DomainError.CommonError.NotFound(nameof(Brand)));
        }

        return Result<BrandModel>.Success(brand);
    }
}