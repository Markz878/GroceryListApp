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
        IPage page = await WebApplicationFactoryFixture.GetPlaywrightPage();
        await page.Locator("h2:has-text(\"Grocery List Helper\")").WaitForAsync();
    }
}
