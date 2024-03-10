using GroceryListHelper.Core.Domain.CartProducts;
using System.Text.Json;

namespace GroceryListHelper.E2ETests.TestScenarios;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class LocalStorageCartTests : IAsyncLifetime
{
    private readonly WebApplicationFactoryFixture server;
    private IBrowserContext browserContext = default!;
    private IPage page = default!;

    public LocalStorageCartTests(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
    {
        server.CreateDefaultClient();
        server.TestOutputHelper = testOutputHelper;
        this.server = server;
    }

    public async Task InitializeAsync()
    {
        browserContext = await server.GetNewBrowserContext();
        page = await browserContext.GotoPage(server.BaseUrl);
    }

    [Fact]
    public async Task AddValidProductToCart()
    {
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page.AddProductToCart(productName, productAmount, productPrice);
        string cartProductsJson = await page.EvaluateAsync<string>("localStorage.getItem('cartProducts')");
        CartProduct[]? models = JsonSerializer.Deserialize<CartProduct[]>(cartProductsJson, server.JsonOptions);
        ArgumentNullException.ThrowIfNull(models);
        Assert.Equal(productName, models[0].Name);
        Assert.Equal(productAmount, models[0].Amount);
        Assert.Equal(productPrice, models[0].UnitPrice);
        Assert.False(models[0].IsCollected);
    }

    [Fact]
    public async Task AddEmptyProductNameToCart_ShowsModalWithMessage_WithoutAddingProduct()
    {
        await page.AddProductToCart("", 2, 2.9);
        await page.WaitForSelectorAsync("text=Product name not given");
        Assert.Empty(await page.GetRow(0).ElementHandlesAsync());
    }

    [Fact]
    public async Task AddNegativeProductAmountToCart_ShowsModalWithMessage_WithoutAddingProduct()
    {
        await page.AddProductToCart("Maito", -2, 2.9);
        await page.WaitForSelectorAsync("text=Amount must be between 0 and 10 000");
        Assert.Empty(await page.GetRow(0).ElementHandlesAsync());
    }

    [Fact]
    public async Task AddNegativePriceToCart_ShowsModalWithMessage_WithoutAddingProduct()
    {
        await page.AddProductToCart("Maito", 2, -2.9);
        await page.WaitForSelectorAsync("text=Price must be between 0 and 10 000");
        Assert.Empty(await page.GetRow(0).ElementHandlesAsync());
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
        await page.ClickAsync("text=Yes");
        IReadOnlyList<IElementHandle> rows = await page.GetRow(0).ElementHandlesAsync();
        Assert.Empty(rows);
        string cartProductsJson = await page.EvaluateAsync<string>("localStorage.getItem('cartProducts')");
        Assert.Equal("[]", cartProductsJson);
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
        Assert.Equal("2.5", await page.GetItemPrice(0));
        string cartProductsJson = await page.EvaluateAsync<string>("localStorage.getItem('cartProducts')");
        CartProduct[]? models = JsonSerializer.Deserialize<CartProduct[]>(cartProductsJson, server.JsonOptions);
        ArgumentNullException.ThrowIfNull(models);
        Assert.Equal(2, models[0].Amount);
        Assert.Equal(2.5, models[0].UnitPrice);
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
        string cartProductsJson = await page.EvaluateAsync<string>("localStorage.getItem('cartProducts')");
        CartProduct[]? models = JsonSerializer.Deserialize<CartProduct[]>(cartProductsJson, server.JsonOptions);
        ArgumentNullException.ThrowIfNull(models);
        Assert.Equal(productCount - 1, models.Length);
    }


    public async Task DisposeAsync()
    {
        await browserContext.DisposeAsync();
    }
}
