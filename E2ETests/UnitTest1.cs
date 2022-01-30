using GroceryListHelper.Server;
using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace E2ETests;

public class WebServerTests : IClassFixture<WebHostServerFixture<Startup>>
{
    private readonly WebHostServerFixture<Startup> _server;

    public WebServerTests(WebHostServerFixture<Startup> server)
    {
        _server = server;
    }

    [Fact]
    public async Task PageTitleContainsProductName()
    {
        using IPlaywright playwright = await Playwright.CreateAsync();
        IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            //Headless = false,
            //SlowMo = 5000,
        });
        IBrowserContext browserContext = await browser.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true,
        });
        IPage page = await browserContext.NewPageAsync();
        IResponse response = await page.GotoAsync(_server.RootUri.AbsoluteUri);
        await page.Locator("h2:has-text(\"Grocery List Helper\")").WaitForAsync();
        Assert.True(response.Ok);
    }
}
