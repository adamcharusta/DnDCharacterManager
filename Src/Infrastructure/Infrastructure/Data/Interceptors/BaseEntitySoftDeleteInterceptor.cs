using DnDCharacterManager.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors;

public class BaseEntitySoftDeleteInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplySoftDelete(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var user = currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;

            entry.Entity.IsDeleted = true;
            entry.Entity.LastModified = now;
            entry.Entity.LastModifiedBy = user;

            entry.Property(e => e.Created).IsModified = false;
            entry.Property(e => e.CreatedBy).IsModified = false;
        }
    }
}
