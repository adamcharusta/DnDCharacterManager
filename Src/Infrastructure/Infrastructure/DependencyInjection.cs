using Ardalis.GuardClauses;
using DnDCharacterManager.Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        var connectionString = configurationManager.GetConnectionString("DefaultConnection");
        Guard.Against.NullOrEmpty(connectionString, "DefaultConnection");

        services.AddHealthChecks().AddSqlServer(connectionString);

        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            opt.UseSqlServer(connectionString);
        });

        services.AddScoped<ISaveChangesInterceptor, BaseEntityAuditInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, BaseEntitySoftDeleteInterceptor>();

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<AppDbContextInitializer>();

        services.AddSingleton(TimeProvider.System);

        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
