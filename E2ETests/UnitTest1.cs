using GroceryListHelper.Server;
using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace E2ETests;

public class WebServerTests : IClassFixture<WebApplicationFactoryFixture>
{
    public WebServerTests(WebApplicationFactoryFixture server)
    {
        server.CreateDefaultClient();
    }

    [Fact]
    public async Task PageTitleContainsProductName()
    {
        using IPlaywright playwright = await Playwright.CreateAsync();
        IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = false,
            SlowMo = 5000,
        });
        IBrowserContext browserContext = await browser.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true,
        });
        IPage page = await browserContext.NewPageAsync();
        IResponse response = await page.GotoAsync("https://localhost:5001");
        await page.Locator("h2:has-text(\"Grocery List Helper\")").WaitForAsync();
        Assert.True(response.Ok);
    }
}
