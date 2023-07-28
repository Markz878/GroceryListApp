using E2ETests.Infrastructure;
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
        IElementHandle? element = await page.GetRow(0).ElementHandleAsync();
        Assert.NotNull(element);
    }

    [Theory]
    [InlineData(5, 4, 0)]
    [InlineData(5, 1, 4)]
    [InlineData(5, 0, 4)]
    public async Task AddValidProducts_ReorderProducts_ProductsAreInCorrectOrder(int productCount, int moveItemIndex, int toTargetIndex)
    {
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", i + 1, i * 1.5 + 0.5);
        }
        await page.ClickReorderButton(moveItemIndex);
        await page.ClickReorderButton(toTargetIndex);
        await Task.Delay(100);
        int sourceTop = await page.GetRowTopPixels(moveItemIndex);
        int targetTop = await page.GetRowTopPixels(toTargetIndex);
        if (toTargetIndex > 0)
        {
            Assert.True(sourceTop > targetTop);
        }
        else
        {
            Assert.True(sourceTop < targetTop);
        }
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
        IReadOnlyList<IElementHandle> rows = await page.GetRow(0).ElementHandlesAsync();
        Assert.Empty(rows);
    }

    [Fact]
    public async Task AddProduct_EditProperties_ShouldChangeValues()
    {
        await page.AddProductToCart($"Product", 1, 1.5);
        await page.ClickEditButton(0);
        await page.FillEditAmount(0, 2);
        await page.FillEditPrice(0, 2.50);
        await page.ClickSubmitEditButton(0);
        Assert.Equal("2", await page.GetItemAmount(0));
        Assert.Equal("2.50", await page.GetItemPrice(0));
    }

    [Fact]
    public async Task AddProducts_ClickProductDeleteButton_ShouldRemoveItem()
    {
        int productCount = 3;
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", i + 1, i * 1.5 + 0.5);
        }
        await page.ClickDeleteButton(productCount / 2);
        IElementHandle? element = await page.QuerySelectorAsync($"text=Product{productCount / 2}");
        Assert.Null(element);
    }

    public async Task InitializeAsync()
    {
        browserContext = await server.GetNewBrowserContext(fakeAuth);
        page = await browserContext.GotoPage(server.BaseUrl, true);
    }
    public async Task DisposeAsync()
    {
        await browserContext.DisposeAsync();
    }
}
