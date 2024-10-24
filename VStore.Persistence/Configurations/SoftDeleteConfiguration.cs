using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VStore.Domain.Abstractions.Entities;

namespace VStore.Persistence.Configurations;

public static class SoftDeleteConfiguration
{
    public static void ApplySoftDeleteFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType)) continue;
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var filter = Expression.Lambda(
                Expression.Equal(
                    Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)),
                    Expression.Constant(false)
                ),
                parameter
            );
            // apply global filter on all entities that implement ISoftDelete
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            // create index on IsDeleted column for performance
            modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(ISoftDelete.IsDeleted));
        }
    }
}