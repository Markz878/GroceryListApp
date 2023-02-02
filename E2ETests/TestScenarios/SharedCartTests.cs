using E2ETests.Infrastructure;
using GroceryListHelper.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests.TestScenarios;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class SharedCartTests : IAsyncLifetime
{
    private readonly WebApplicationFactoryFixture server;
    private readonly FakeAuthInfo fakeAuth1 = new("Test User 1", "test_user1@email.com", Guid.NewGuid());
    private readonly FakeAuthInfo fakeAuth2 = new("Test User 2", "test_user2@email.com", Guid.NewGuid());
    private IBrowserContext browserContext1 = default!;
    private IPage page1 = default!;
    private IBrowserContext browserContext2 = default!;
    private IPage page2 = default!;

    public SharedCartTests(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
    {
        server.CreateDefaultClient();
        server.TestOutputHelper = testOutputHelper;
        this.server = server;
    }

    [Fact]
    public async Task AddValidProductToCart()
    {
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page1.AddProductToCart(productName, productAmount, productPrice);
        IElementHandle? element = await page2.QuerySelectorAsync("#item-name-0");
        ArgumentNullException.ThrowIfNull(element);
        string elementText = await element.InnerTextAsync();
        Assert.Equal(productName, elementText);
    }

    [Fact]
    public async Task DeleteProductFromCart()
    {
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page1.AddProductToCart(productName, productAmount, productPrice);
        await page2.ClickAsync("#delete-product-button-0");
        IElementHandle? element = await page1.QuerySelectorAsync("#item-name-0");
        Assert.Null(element);
    }

    [Fact]
    public async Task UpdateProductFromCart()
    {
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page1.AddProductToCart(productName, productAmount, productPrice);
        await page2.ClickAsync("#edit-product-button-0");
        await page2.FillAsync("#edit-item-amount-input-0", (productAmount + 1).ToString());
        await page2.FillAsync("#edit-item-unitprice-input-0", "3.1");
        await page2.ClickAsync("#update-product-button-0");
        IElementHandle? amountElement = await page1.QuerySelectorAsync("#item-amount-0");
        ArgumentNullException.ThrowIfNull(amountElement);
        string amountText = await amountElement.InnerTextAsync();
        Assert.Equal("3", amountText);
        IElementHandle? priceElement = await page1.QuerySelectorAsync("#item-unitprice-0");
        ArgumentNullException.ThrowIfNull(priceElement);
        string priceText = await priceElement.InnerTextAsync();
        Assert.Equal("3.1", priceText);
    }

    [Fact]
    public async Task LeaveCartHostedByOther()
    {
        await page2.ClickAsync("#exit-cart-btn");
        await Task.Delay(500);
        IElementHandle? messageElement = await page1.QuerySelectorAsync($"p:has-text('{fakeAuth2.Email} has left the group.')");
        Assert.NotNull(messageElement);
    }

    [Fact]
    public async Task LeaveCartSelfHosted()
    {
        await page1.ClickAsync("#exit-share-cart-btn");
        await Task.Delay(500);
        IElementHandle? messageElement = await page2.QuerySelectorAsync($"h4:has-text('Cart session ended by host {fakeAuth1.Email}.')");
        Assert.NotNull(messageElement);
    }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = server.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        await db.Database.EnsureCreatedAsync();
        browserContext1 = await server.GetNewBrowserContext(fakeAuth1);
        page1 = await browserContext1.GotoPage(server.BaseUrl, true);
        browserContext2 = await server.GetNewBrowserContext(fakeAuth2);
        page2 = await browserContext2.GotoPage(server.BaseUrl, true);
        await ShareCartMethods.StartShare(page1, page2, fakeAuth1.Email, fakeAuth2.Email);
    }
    public async Task DisposeAsync()
    {
        await browserContext1.DisposeAsync();
        await browserContext2.DisposeAsync();
        using IServiceScope scope = server.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        await db.Database.EnsureDeletedAsync();
    }
}
