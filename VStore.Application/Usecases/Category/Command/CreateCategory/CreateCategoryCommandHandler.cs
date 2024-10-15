using AutoMapper;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Category.Command.CreateCategory;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var isExistCategoryName = await _categoryRepository.AnyAsync(x => x.Name == request.Name, cancellationToken);
        if (isExistCategoryName)
        {
            return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(Domain.Entities.Category.Name)));
        }

        if (request.ParentId != 0)
        {
            var parentId = await _categoryRepository.FindByIdAsync(request.ParentId, cancellationToken);
            if (parentId == null)
            {
                return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Category.ParentId)));
            }
        }

        var category = _mapper.Map<Domain.Entities.Category>(request);
        _categoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        return Result.Success();
    }
}