using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace GroceryListHelper.IntegrationTests.Hooks;

public class WebApplicationFactoryFixture : WebApplicationFactory<Server.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseUrls("https://localhost:5001");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost fixtureHost = builder.Build();
        builder.ConfigureWebHost(b => b.UseKestrel());
        IHost host = builder.Build();
        host.Start();
        return fixtureHost;
    }

    public static async Task<IPage> GetPlaywrightPage(string path = "")
    {
        IPlaywright playwright = await Playwright.CreateAsync();
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
        IResponse response = await page.GotoAsync("https://localhost:5001" + path);
        return page;
    }
}
