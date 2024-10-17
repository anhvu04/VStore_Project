using Microsoft.EntityFrameworkCore;
using VStore.Application.Abstractions.MediatR;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Errors.DomainErrors;
using VStore.Domain.Shared;

namespace VStore.Application.Usecases.Category.Command.DeleteCategory;

public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure(DomainError.CommonError.NotFound(nameof(Domain.Entities.Category)));
        }

        var subCategory = await _categoryRepository.FindAll(x => x.ParentId == category.Id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (subCategory != null)
        {
            return Result.Failure(DomainError.Category.HasSubCategory);
        }

        category.IsActive = false;
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(true, cancellationToken);
        return Result.Success();
    }
}