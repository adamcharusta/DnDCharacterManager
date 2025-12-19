using DnDCharacterManager.Tests.Components.Common;
using DnDCharacterManager.Web.Components.Layout;

namespace DnDCharacterManager.Tests.Components.Components;

public class NavMenuTests : ComponentTestFactory
{
    [Fact]
    public void Should_show_logout_button_when_user_is_authorized()
    {
        var cut = Render<NavMenu>();

        cut.Markup.Should().Contain("Wyloguj");
    }

    [Fact]
    public void Should_show_login_button_when_user_is_authorized()
    {
        Auth.SetNotAuthorized();

        var cut = Render<NavMenu>();

        cut.Markup.Should().Contain("Login");
    }
}
