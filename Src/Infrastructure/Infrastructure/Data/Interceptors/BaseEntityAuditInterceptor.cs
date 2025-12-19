using DnDCharacterManager.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public sealed class BaseEntityAuditInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var user = currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = now;
                entry.Entity.CreatedBy = user;

                entry.Entity.LastModified = now;
                entry.Entity.LastModifiedBy = user;

                entry.Entity.IsDeleted = false;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.Created).IsModified = false;
                entry.Property(e => e.CreatedBy).IsModified = false;

                entry.Entity.LastModified = now;
                entry.Entity.LastModifiedBy = user;
            }
        }
    }
}
