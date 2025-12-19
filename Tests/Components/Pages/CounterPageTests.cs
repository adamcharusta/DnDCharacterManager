using DnDCharacterManager.Contracts.Notifications;
using DnDCharacterManager.Tests.Components.Common;
using DnDCharacterManager.Web.Components.Pages;

public class CounterPageTests : ComponentTestFactory
{
    [Fact]
    public void Clicking_button_should_increment_counter()
    {
        var cut = Render<Counter>();

        cut.Find("p[role='status']").TextContent.Should().Contain("Current count: 0");

        cut.Find("button").Click();

        cut.Find("p[role='status']").TextContent.Should().Contain("Current count: 1");
    }

    [Fact]
    public void Submitting_form_with_valid_email_should_trigger_bus_send()
    {
        var cut = Render<Counter>();

        var emailInput = cut.Find("form input");
        emailInput.Input("test@example.com");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Bus.Received(1).SendAsync(
                Arg.Is((SendEmail m) =>
                    m.To == "test@example.com" &&
                    m.Subject == "Welcome" &&
                    m.Body == "Hello from Wolverine"
                )
            );
        });
    }

    [Fact]
    public void Submitting_form_with_invalid_email_should_not_trigger_bus_send()
    {
        var cut = Render<Counter>();

        var emailInput = cut.Find("form input");
        emailInput.Input("not-an-email");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Bus.DidNotReceive().SendAsync(Arg.Any<SendEmail>());
        });

        cut.Markup.Should().Contain("The email address is invalid");
    }
}
