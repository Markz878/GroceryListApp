using GroceryListHelper.Shared;
using Microsoft.Playwright;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests.TestScenarios;

[Collection(nameof(WebApplicationFactoryCollection))]
public class LocalStorageCartTests
{
    private readonly WebApplicationFactoryFixture fixture;

    public LocalStorageCartTests(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
    {
        server.CreateDefaultClient();
        server.TestOutputHelper = testOutputHelper;
        fixture = server;
    }

    [Fact]
    public async Task AddValidProductToCart()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page.AddProductToCart(productName, productAmount, productPrice);
        string cartProductsJson = await page.EvaluateAsync<string>("localStorage.getItem('cartProducts')");
        CartProductCollectable[] models = JsonSerializer.Deserialize<CartProductCollectable[]>(cartProductsJson);
        Assert.True(models[0].Name == productName);
        Assert.True(models[0].Amount == productAmount);
        Assert.True(models[0].UnitPrice == productPrice);
        Assert.True(models[0].IsCollected == false);
    }

    [Fact]
    public async Task AddEmptyProductNameToCart_ShowsModalWithMessage_WithoutAddingProduct()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        await page.AddProductToCart("", 2, 2.9);
        await page.WaitForSelectorAsync("h4:has-text(\"'Name' must not be empty.\")");
        Assert.Null(await page.QuerySelectorAsync("td:has-text(\"2.9\")"));
    }

    [Fact]
    public async Task AddNegativeProductAmountToCart_ShowsModalWithMessage_WithoutAddingProduct()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        await page.AddProductToCart("Maito", -2, 2.9);
        await page.WaitForSelectorAsync("h4:has-text(\"'Amount' must be greater than or equal to '0'.\")");
        Assert.Null(await page.QuerySelectorAsync("td:has-text(\"Maito\")"));
    }

    [Fact]
    public async Task AddNegativePriceToCart_ShowsModalWithMessage_WithoutAddingProduct()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        await page.AddProductToCart("Maito", 2, -2.9);
        await page.WaitForSelectorAsync("h4:has-text(\"'Unit Price' must be greater than or equal to '0'.\")");
        Assert.Null(await page.QuerySelectorAsync("td:has-text(\"Maito\")"));
    }

    [Theory]
    [InlineData(4,0)]
    [InlineData(3,1)]
    [InlineData(2,3)]
    [InlineData(1,4)]
    [InlineData(0,4)]
    public async Task Add5ValidProducts_ReorderLastTo_ProductsAreInCorrectOrder(int source, int target)
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        for (int i = 0; i < 5; i++)
        {
            await page.AddProductToCart($"Product{i}", (i+1), i*1.5+0.5);
        }
        await page.ClickAsync($"#reorder-button-{source}");
        await page.ClickAsync($"#reorder-button-{target}");
        IElementHandle movedProductNameElement = await page.QuerySelectorAsync($"#item-name-{target}");
        string movedProductName = await movedProductNameElement.InnerTextAsync();
        Assert.Equal($"Product{source}", movedProductName);
    }
}
