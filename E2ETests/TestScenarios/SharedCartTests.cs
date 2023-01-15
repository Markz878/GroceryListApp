using GroceryListHelper.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests.TestScenarios;

[Collection(nameof(SharedWebApplicationFactoryCollection))]
public sealed class SharedCartTests : IAsyncLifetime
{
    private readonly AuthorizedWebApplicationFactoryFixture server1;
    private readonly AuthorizedWebApplicationFactoryFixture2 server2;

    public SharedCartTests(SharedWebApplicationFactoryFixture servers, ITestOutputHelper testOutputHelper)
    {
        server1 = servers.Server1;
        server2 = servers.Server2;
        server1.CreateDefaultClient();
        server2.CreateDefaultClient();
        server1.TestOutputHelper = testOutputHelper;
        server2.TestOutputHelper = testOutputHelper;
    }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = server1.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
    public async Task DisposeAsync()
    {
        using IServiceScope scope = server1.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        await db.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task AddValidProductToCart()
    {
        await using IBrowserContext BrowserContext1 = await server1.BrowserInstance.GetNewBrowserContext();
        IPage page1 = await BrowserContext1.GotoPage(server1.BaseUrl, true);
        await using IBrowserContext BrowserContext2 = await server2.BrowserInstance.GetNewBrowserContext();
        IPage page2 = await BrowserContext2.GotoPage(server2.BaseUrl, true);
        await ShareCartMethods.StartShare(page1, page2);
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page1.AddProductToCart(productName, productAmount, productPrice);
        await Task.Delay(1000);
        IElementHandle element = await page2.QuerySelectorAsync("#item-name-0");
        string page2Text = await page2.TextContentAsync("#content");
        Assert.NotNull(element);
    }


    //[Fact]
    //public async Task DeleteProductFromCart()
    //{
    //    await using IBrowserContext BrowserContext1 = await fixture.BrowserInstance.GetNewBrowserContext();
    //    IPage page1 = await BrowserContext1.GotoPage();
    //    await using IBrowserContext BrowserContext2 = await fixture.BrowserInstance.GetNewBrowserContext();
    //    IPage page2 = await BrowserContext2.GotoPage();
    //    await ShareCartMethods.StartShare(page1, page2);
    //    string productName = "Maito";
    //    int productAmount = 2;
    //    double productPrice = 2.9;
    //    await page1.AddProductToCart(productName, productAmount, productPrice);
    //    await Task.Delay(1000);
    //    IElementHandle element1 = await page2.QuerySelectorAsync("#item-name-0");
    //    Assert.NotNull(element1);
    //    await page1.ClickAsync("#delete-product-button-0");
    //    await Task.Delay(1000);
    //    IElementHandle element2 = await page2.QuerySelectorAsync("#item-name-0");
    //    Assert.Null(element2);
    //}
}
