using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VStore.Domain.Abstractions;
using VStore.Domain.Abstractions.Entities;
using VStore.Domain.Exceptions;
using static System.String;

namespace VStore.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public UnitOfWork(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _dbContext = dbContext;
        _contextAccessor = contextAccessor;
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

    public async Task SaveChangesAsync(bool isUserTracking, bool isDateTracking,
        CancellationToken cancellationToken = default)
    {
        await using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            if (isUserTracking)
            {
                UserTrackingEntities();
            }

            if (isDateTracking)
            {
                DateTrackingEntities();
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await dbContextTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await dbContextTransaction.RollbackAsync(cancellationToken);
            throw new DataSavingException(ex.Message + "\n" + ex.InnerException?.Message);
        }
    }

    public async Task SaveChangesAsync(bool isSoftDelete, CancellationToken cancellationToken = default)
    {
        await using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            if (isSoftDelete)
            {
                SoftDeleteEntities();
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await dbContextTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await dbContextTransaction.RollbackAsync(cancellationToken);
            throw new DataSavingException(ex.Message + "\n" + ex.InnerException?.Message);
        }
    }

    private void UserTrackingEntities()
    {
        var context = _contextAccessor.HttpContext;
        if (context == null)
        {
            return;
        }

        var userId = Guid.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? Empty);
        if (userId == default)
        {
            return;
        }

        var entries = _dbContext.ChangeTracker.Entries<IUserTracking>().Where(x =>
            x.State is EntityState.Added or EntityState.Modified);
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                // Set CreatedBy to current user only if it's not already set
                if (entry.Entity.CreatedBy == default)
                {
                    entry.Entity.CreatedBy = userId;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedBy = userId;
            }
        }
    }

    private void DateTrackingEntities()
    {
        var entries = _dbContext.ChangeTracker.Entries<IDateTracking>().Where(x =>
            x.State is EntityState.Added or EntityState.Modified);
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                // Set CreatedDate to current date only if it's not already set
                if (entry.Entity.CreatedDate == default)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedDate = DateTime.UtcNow;
            }
        }
    }

    private void SoftDeleteEntities()
    {
        var entries = _dbContext.ChangeTracker.Entries<ISoftDelete>().Where(x =>
            x.State is EntityState.Deleted);
        foreach (var entry in entries)
        {
            entry.Entity.IsDeleted = true;
            entry.State = EntityState.Modified;
            entry.Entity.DeletedDate = DateTime.UtcNow;
        }
    }

    public DbContext GetDbContext()
    {
        return _dbContext;
    }
}