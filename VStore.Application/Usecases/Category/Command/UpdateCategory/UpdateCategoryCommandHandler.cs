using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Category.Command.UpdateCategory;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Category)));
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            var isExistCategoryName =
                await _categoryRepository.AnyAsync(x => x.Name == request.Name && x.Id != request.Id,
                    cancellationToken);
            if (isExistCategoryName)
            {
                return Result.Failure(DomainError.CommonError.AlreadyExists(nameof(Domain.Entities.Category.Name)));
            }

            category.Name = request.Name;
        }

        if (request.ParentId != 0)
        {
            var categoryList = await _categoryRepository.FindAll().ToListAsync(cancellationToken);
            if (categoryList.All(x => x.Id != request.ParentId))
            {
                return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Category.ParentId)));
            }

            if (_categoryRepository.IsAncestorOf(request.Id, request.ParentId, categoryList))
            {
                return Result.Failure(DomainError.Category.InvalidParentCategory);
            }

            category.ParentId = request.ParentId;
        }
        
        category.Description = request.Description ?? category.Description;
        category.IsActive = request.IsActive ?? category.IsActive;
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(true, true, cancellationToken);
        return Result.Success();
    }
}