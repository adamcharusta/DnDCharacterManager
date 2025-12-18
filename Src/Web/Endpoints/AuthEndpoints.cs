using Application.Common.Dtos;
using DnDCharacterManager.Application.Auth.Commands.LoginUser;
using DnDCharacterManager.Application.Auth.Commands.LogoutUserQuery;
using DnDCharacterManager.Application.Auth.Commands.RegisterUser;
using Wolverine;

namespace DnDCharacterManager.Web.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            HttpContext ctx,
            IMessageBus bus) =>
        {
            var form = await ctx.Request.ReadFormAsync();

            var email = form["Email"].ToString();
            var password = form["Password"].ToString();
            var returnUrl = form["ReturnUrl"].ToString();

            var result = await bus.InvokeAsync<LoginResult>(new LoginUserQuery(email, password));

            if (!result.Succeeded)
            {
                return Results.Redirect("/account/login?error=1");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
            {
                return Results.Redirect(returnUrl);
            }

            return Results.Redirect("/");
        });

        app.MapPost("/auth/register", async (
            HttpContext ctx,
            IMessageBus bus) =>
        {
            var form = await ctx.Request.ReadFormAsync();

            var email = form["Email"].ToString();
            var password = form["Password"].ToString();

            var result = await bus.InvokeAsync<RegisterResult>(new RegisterUserCommand(email, password));

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Redirect("/");
        });

        app.MapPost("/auth/logout", async (IMessageBus bus) =>
        {
            await bus.InvokeAsync(new LogoutUserQuery());
            return Results.Redirect("/account/login");
        });

        return app;
    }
}
