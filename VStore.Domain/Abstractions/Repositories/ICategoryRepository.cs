using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface ICategoryRepository : IRepositoryBase<Category, int>
{
    bool IsAncestorOf(int childId, int ancestorId, List<Category> categories);
}