using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests.TestScenarios;

[Collection(nameof(WebApplicationFactoryCollection))]
public class SharedCartTests
{
    private readonly WebApplicationFactoryFixture fixture;

    public SharedCartTests(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
    {
        server.CreateDefaultClient();
        server.TestOutputHelper = testOutputHelper;
        fixture = server;
    }

    [Fact]
    public async Task AddValidProductToCart()
    {
        await using IBrowserContext BrowserContext1 = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page1 = await BrowserContext1.GotoPage();
        await using IBrowserContext BrowserContext2 = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page2 = await BrowserContext2.GotoPage();
        await ShareCartMethods.StartShare(page1, page2);
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page1.AddProductToCart(productName, productAmount, productPrice);
        IElementHandle element = await page2.QuerySelectorAsync("#item-name-0");
        Assert.NotNull(element);
    }

    [Fact]
    public async Task DeleteProductFromCart()
    {
        await using IBrowserContext BrowserContext1 = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page1 = await BrowserContext1.GotoPage();
        await using IBrowserContext BrowserContext2 = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page2 = await BrowserContext2.GotoPage();
        await ShareCartMethods.StartShare(page1, page2);
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page1.AddProductToCart(productName, productAmount, productPrice);
        IElementHandle element1 = await page2.QuerySelectorAsync("#item-name-0");
        Assert.NotNull(element1);
        await page1.ClickAsync("#delete-product-button-0");
        IElementHandle element2 = await page2.QuerySelectorAsync("#item-name-0");
        Assert.Null(element2);
    }
}
