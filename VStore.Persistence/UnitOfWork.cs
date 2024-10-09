using Microsoft.EntityFrameworkCore;
using VStore.Domain.Abstractions;
using VStore.Domain.Exceptions;

namespace VStore.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await dbContextTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await dbContextTransaction.RollbackAsync(cancellationToken);
            throw new DataSavingException(ex.Message + "\n" + ex.InnerException?.Message);
        }
    }

    public DbContext GetDbContext()
    {
        return _dbContext;
    }
}