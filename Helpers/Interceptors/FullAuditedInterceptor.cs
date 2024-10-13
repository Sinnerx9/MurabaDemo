using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MurabaDemo.Database.Tables.Infrastructure;
using MurabaDemo.Helpers.Extensions;

namespace MurabaDemo.Helpers.Interceptors;

public class FullAuditedInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        HandleFullAudited(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        HandleFullAudited(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }


    private void HandleFullAudited(DbContext? context)
    {
        if (context == null) return;


        var userId = httpContextAccessor.GetId<Guid>();
        if (userId == null)
            return;
        var entries = context.ChangeTracker.Entries()
            .Where(x => x.Entity is FullAuditedEntitiy<Guid> &&
                        (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (FullAuditedEntitiy<Guid>)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.createdById = userId.Value;
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entity.isDeleted == true)
                    entity.deletedById = userId;
                else
                    entity.updatedById = userId;
            }
        }
    }
}