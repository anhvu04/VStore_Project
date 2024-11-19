using System.Linq.Expressions;

namespace VStore.Domain.Abstractions.Repositories;

public interface IRepositoryBase<TEntity, in TKey> where TEntity : EntityBase<TKey>
{
    Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includes);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void RemoveRange(IEnumerable<TEntity> entities);
    void UpdateRange(IEnumerable<TEntity> entities);
}