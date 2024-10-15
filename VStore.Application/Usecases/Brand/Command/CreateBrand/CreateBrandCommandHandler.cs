using AutoMapper;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Brand.Command.CreateBrand;

public class CreateBrandCommandHandler : ICommandHandler<CreateBrandCommand>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBrandCommandHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brandName = await _brandRepository.FindSingleAsync(x => x.Name == request.Name, cancellationToken);
        if (brandName != null)
        {
            return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(Domain.Entities.Brand.Name)));
        }

        var brand = _mapper.Map<Domain.Entities.Brand>(request);
        _brandRepository.Add(brand);
        await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        return Result.Success();
    }
}