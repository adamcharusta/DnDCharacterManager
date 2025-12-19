using System.Linq.Expressions;
using System.Reflection;
using DnDCharacterManager.Application.Common.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IAppDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ApplySoftDeleteQueryFilter(builder);
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType == null)
            {
                continue;
            }

            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(BuildIsDeletedFilter(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression BuildIsDeletedFilter(Type clrType)
    {
        var param = Expression.Parameter(clrType, "e");
        var prop = Expression.Property(param, nameof(BaseEntity.IsDeleted));
        var body = Expression.Equal(prop, Expression.Constant(false));
        return Expression.Lambda(body, param);
    }
}
