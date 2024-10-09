using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Repositories;

namespace VStore.Persistence.Repositories;

public abstract class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>
    where TEntity : EntityBase<TKey>
{
    private readonly ApplicationDbContext _context;

    protected RepositoryBase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await FindAll(x => x.Id!.Equals(id), includes)
            .AsTracking()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await FindAll(predicate, includes)
            .AsTracking()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>().AsNoTracking();
        if (includes.Length != 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query;
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await FindAll(predicate, includes)
            .AnyAsync(cancellationToken);
    }

    public void Add(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().AddRange(entities);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
    }
}