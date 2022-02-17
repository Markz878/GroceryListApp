using GroceryListHelper.Server;
using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests;

[Collection(nameof(WebApplicationFactoryCollection))]
public class WebServerTests2
{
    public WebServerTests2(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
    {
        server.CreateDefaultClient();
        server.TestOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task UserCanRegister()
    {
        using IPlaywright PlaywrightInstance = await Playwright.CreateAsync();
        await using IBrowser BrowserInstance = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = false,
            SlowMo = 1000,
        });
        await using IBrowserContext BrowserContext = await BrowserInstance.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true,
        });
        IPage page = await BrowserContext.NewPageAsync();
        await page.GotoAsync("https://localhost:5001");
        await page.Locator("h2:has-text(\"Grocery List Helper\")").WaitForAsync();
        await page.ClickAsync("a:has-text(\"Register\")");
        await page.FillAsync("#email", "test@gmail.com");
        await page.FillAsync("#password", "Hablahattu51");
        await page.FillAsync("#confirm-password", "Hablahattu51");
        await page.ClickAsync("button:has-text(\"Register\")");
    }

    [Fact]
    public async Task UserCanAddProductToCart()
    {
        using IPlaywright PlaywrightInstance = await Playwright.CreateAsync();
        await using IBrowser BrowserInstance = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = false,
            SlowMo = 1000,
        });
        await using IBrowserContext BrowserContext = await BrowserInstance.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true,
        });
        IPage page = await BrowserContext.NewPageAsync();
        await page.GotoAsync("https://localhost:5001");
        await page.FillAsync("#newproduct-name-input", "Maito");
        await page.FillAsync("#newproduct-amount-input", "2");
        await page.FillAsync("#newproduct-price-input", "2.9");
        await page.ClickAsync("button:has-text(\"Add\")");
    }
}
