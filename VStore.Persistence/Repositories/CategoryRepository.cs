using Microsoft.EntityFrameworkCore;
using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class CategoryRepository : RepositoryBase<Category, int>, ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public bool IsAncestorOf(int childId, int ancestorId, List<Category> categories)
    {
        var currentParentId = categories.FirstOrDefault(x => x.Id == childId)?.ParentId;
        while (currentParentId.HasValue)
        {
            if (currentParentId.Value == ancestorId)
            {
                return true;
            }

            currentParentId = categories.FirstOrDefault(x => x.Id == currentParentId.Value)?.ParentId;
        }

        return false;
    }

    public async Task<bool> IsCategoryHasProductAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AnyAsync(x => x.CategoryId == categoryId, cancellationToken);
    }
}