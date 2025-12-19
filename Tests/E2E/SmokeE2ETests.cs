namespace DnDCharacterManager.Tests.E2E;

public class SmokeE2ETests
{
    [Fact]
    public async Task Home_page_should_load_and_have_a_title()
    {
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return;
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser =
            await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        var page = await browser.NewPageAsync();
        await page.GotoAsync(baseUrl);

        var title = await page.TitleAsync();

        title.Should().NotBeNullOrWhiteSpace();
    }
}
