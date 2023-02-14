using E2ETests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests.TestScenarios;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class ServerStorageCartTests : IAsyncLifetime
{
    private readonly WebApplicationFactoryFixture server;
    private readonly FakeAuthInfo fakeAuth = new("Test User", "test_user@email.com", Guid.NewGuid());
    private IBrowserContext browserContext = default!;
    private IPage page = default!;

    public ServerStorageCartTests(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
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
        await page.AddProductToCart(productName, productAmount, productPrice);
        IElementHandle? element = await page.QuerySelectorAsync("#item-name-0");
        Assert.NotNull(element);
    }

    [Fact]
    public async Task AddEmptyProductNameToCart_ShowsModalWithMessage_WithoutAddingProduct()
    {
        await page.AddProductToCart("", 2, 2.9);
        await page.WaitForSelectorAsync("text='Name' must not be empty.");
        Assert.Null(await page.QuerySelectorAsync("td:has-text(\"2.9\")"));
    }

    [Theory]
    [InlineData(5, 1, 4)]
    [InlineData(5, 0, 4)]
    public async Task AddValidProducts_ReorderProducts_ProductsAreInCorrectOrder(int productCount, int moveItemIndex, int toTargetIndex)
    {
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", i + 1, i * 1.5 + 0.5);
        }
        await page.ClickAsync($"#reorder-button-{moveItemIndex}");
        await page.ClickAsync($"#reorder-button-{toTargetIndex}");
        await Task.Delay(100);
        IElementHandle? movedProductNameElement = await page.QuerySelectorAsync($"#item-name-{toTargetIndex}");
        ArgumentNullException.ThrowIfNull(movedProductNameElement);
        string movedProductName = await movedProductNameElement.InnerTextAsync();
        Assert.Equal($"Product{moveItemIndex}", movedProductName);
    }

    [Fact]
    public async Task AddValidProducts_ClickClearCart_CartShouldBeEmpty()
    {
        int productCount = 3;
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", i + 1, i * 1.5 + 0.5);
        }
        await page.ClickAsync("text=Clear cart");
        IElementHandle? movedProductNameElement = await page.QuerySelectorAsync("#item-name-0");
        Assert.Null(movedProductNameElement);
    }

    [Fact]
    public async Task AddProduct_EditProperties_ShouldChangeValues()
    {
        await page.AddProductToCart($"Product", 1, 1.5);
        await page.ClickAsync("#edit-product-button-0");
        await page.FillAsync("#edit-item-amount-input-0", "2");
        await page.FillAsync("#edit-item-unitprice-input-0", "2.5");
        await page.ClickAsync("#update-product-button-0");
        Assert.Equal("2", await page.InnerTextAsync("#item-amount-0"));
        Assert.Equal("2.5", await page.InnerTextAsync("#item-unitprice-0"));
    }

    [Fact]
    public async Task AddProducts_ClickProductDeleteButton_ShouldRemoveItem()
    {
        int productCount = 3;
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", i + 1, i * 1.5 + 0.5);
        }
        await page.ClickAsync($"#delete-product-button-{productCount / 2}");
        IElementHandle? element = await page.QuerySelectorAsync($"text=Product{productCount / 2}");
        Assert.Null(element);
    }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = server.Services.CreateScope();
        //GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        //await db.Database.EnsureCreatedAsync();
        browserContext = await server.GetNewBrowserContext(fakeAuth);
        page = await browserContext.GotoPage(server.BaseUrl, true);
    }
    public async Task DisposeAsync()
    {
        await browserContext.DisposeAsync();
        //using IServiceScope scope = server.Services.CreateScope();
        //GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        //await db.Database.EnsureDeletedAsync();
    }
}
