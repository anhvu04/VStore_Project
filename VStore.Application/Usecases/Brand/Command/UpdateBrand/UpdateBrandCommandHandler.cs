using AutoMapper;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Brand.Command.UpdateBrand;

public class UpdateBrandCommandHandler : ICommandHandler<UpdateBrandCommand>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateBrandCommandHandler(IUnitOfWork unitOfWork, IBrandRepository brandRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _brandRepository = brandRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.FindByIdAsync(request.Id, cancellationToken);
        if (brand == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Brand)));
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            var brandName = await _brandRepository.FindSingleAsync(x => x.Name == request.Name && x.Id != request.Id,
                cancellationToken);
            if (brandName != null)
            {
                return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(Domain.Entities.Brand.Name)));
            }

            brand.Name = request.Name;
        }

        brand.Description = request.Description ?? brand.Description;
        brand.Logo = request.Logo ?? brand.Logo;
        brand.IsActive = request.IsActive ?? brand.IsActive;
        _brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        return Result.Success();
    }
}