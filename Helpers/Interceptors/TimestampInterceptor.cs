using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MurabaDemo.Database.Tables.Infrastructure;

namespace MurabaDemo.Helpers.Interceptors;

public class TimestampInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        HandleTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        HandleTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }


    private void HandleTimestamps(DbContext? context)
    {
        if (context == null) return;

        var now = DateTime.UtcNow;

        var entries = context.ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity<Guid> &&
                        (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity<Guid>)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.createdAt = now;
                entity.isDeleted = false;
                entity.updatedAt = now;  
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.updatedAt = now; 
            }
        }
    }

}