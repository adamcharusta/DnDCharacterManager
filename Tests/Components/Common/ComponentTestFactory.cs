using System.Security.Claims;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace DnDCharacterManager.Tests.Components.Common;

public abstract class ComponentTestFactory : BunitContext
{
    protected ComponentTestFactory()
    {
        Bus = Substitute.For<IMessageBus>();

        Services.AddMudServices();
        Services.AddSingleton(Bus);

        JSInterop.Mode = JSRuntimeMode.Loose;

        Auth = AddAuthorization();
        Auth.SetAuthorized("test-user");
        Auth.SetClaims(new Claim(ClaimTypes.Name, "test-user"), new Claim(ClaimTypes.Role, "User"));

        NavMan = Services.GetRequiredService<NavigationManager>();
    }

    protected IMessageBus Bus { get; }
    protected NavigationManager NavMan { get; }
    protected BunitAuthorizationContext Auth { get; }
}
