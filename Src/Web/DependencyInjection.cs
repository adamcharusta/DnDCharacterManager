using DnDCharacterManager.Web.Components;
using DnDCharacterManager.Web.Endpoints;
using Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace DnDCharacterManager.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.ConfigureApplicationCookie(o =>
        {
            o.LoginPath = "/account/login";
            o.AccessDeniedPath = "/account/login";
            o.ReturnUrlParameter = "ReturnUrl";

            o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            o.Cookie.SameSite = SameSiteMode.Lax;
        });

        services.AddCascadingAuthenticationState();

        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddMudServices();

        services.AddHttpClient();
        services.AddScoped(sp =>
        {
            var nav = sp.GetRequiredService<NavigationManager>();
            var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
            client.BaseAddress = new Uri(nav.BaseUri);
            return client;
        });

        services.AddAuthorizationCore();

        return services;
    }

    public static async Task<WebApplication> ConfigureWebApplicationAsync(this WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            app.UseExceptionHandler("/Error", true);
            app.UseHsts();
        }
        else
        {
            await app.InitialiseDatabaseAsync();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();

        app.MapStaticAssets();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.MapAuthEndpoints();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }
}
