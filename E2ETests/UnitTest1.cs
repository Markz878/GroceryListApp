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
    public async Task Page_title_equals_Welcome()
    {
        using IPlaywright playwright = await Playwright.CreateAsync();
        IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            //Headless = false,
            //SlowMo = 1000,
        });
        IBrowserContext browserContext = await browser.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true,
        });
        IPage page = await browserContext.NewPageAsync();
        IResponse response = await page.GotoAsync(_server.RootUri.AbsoluteUri);
        string actual = await page.ContentAsync();
        Assert.Contains("Grocery List Helper", actual);
    }
}

public static class Element
{
    public static string ByName(string name)
    {
        return $"[pw-name='{name}']";
    }
}
