using Microsoft.EntityFrameworkCore;

namespace VStore.Domain.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(bool isUserTracking, bool isDateTracking,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(bool isSoftDelete, CancellationToken cancellationToken = default);
    DbContext GetDbContext();
}