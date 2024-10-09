using Microsoft.EntityFrameworkCore;

namespace VStore.Domain.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    DbContext GetDbContext();
}