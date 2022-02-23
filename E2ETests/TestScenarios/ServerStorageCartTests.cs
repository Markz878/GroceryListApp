using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests.TestScenarios;

[Collection(nameof(WebApplicationFactoryCollection))]
public class ServerStorageCartTests
{
    private readonly WebApplicationFactoryFixture fixture;

    public ServerStorageCartTests(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
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
        IElementHandle element = await page.QuerySelectorAsync("#item-name-0");
        Assert.NotNull(element);
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
    [InlineData(5, 4, 0)]
    [InlineData(5, 3, 1)]
    [InlineData(5, 2, 3)]
    [InlineData(5, 1, 4)]
    [InlineData(5, 0, 4)]
    public async Task AddValidProducts_ReorderProducts_ProductsAreInCorrectOrder(int productCount, int moveItemIndex, int toTargetIndex)
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", (i + 1), i * 1.5 + 0.5);
        }
        await page.ClickAsync($"#reorder-button-{moveItemIndex}");
        await page.ClickAsync($"#reorder-button-{toTargetIndex}");
        await Task.Delay(100);
        IElementHandle movedProductNameElement = await page.QuerySelectorAsync($"#item-name-{toTargetIndex}");
        string movedProductName = await movedProductNameElement.InnerTextAsync();
        Assert.Equal($"Product{moveItemIndex}", movedProductName);
    }

    [Fact]
    public async Task AddValidProducts_CheckAllCollected_CartSummmaryShowsAllCollected()
    {
        int productCount = 3;
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", (i + 1), i * 1.5 + 0.5);
        }
        for (int i = 0; i < productCount; i++)
        {
            await page.CheckAsync($"#item-collected-checkbox-{i}");
        }
        IElementHandle movedProductNameElement = await page.QuerySelectorAsync("b:has-text(\"All collected!\")");
        Assert.NotNull(movedProductNameElement);
    }

    [Fact]
    public async Task AddValidProducts_ClickClearCart_CartShouldBeEmpty()
    {
        int productCount = 3;
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", (i + 1), i * 1.5 + 0.5);
        }
        await page.ClickAsync("button:has-text(\"Clear cart\")");
        IElementHandle movedProductNameElement = await page.QuerySelectorAsync("#item-name-0");
        Assert.Null(movedProductNameElement);
        string cartProductsJson = await page.EvaluateAsync<string>("localStorage.getItem('cartProducts')");
        Assert.Null(cartProductsJson);
    }

    [Fact]
    public async Task AddProduct_EditProperties_ShouldChangeValues()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
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
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        for (int i = 0; i < productCount; i++)
        {
            await page.AddProductToCart($"Product{i}", (i + 1), i * 1.5 + 0.5);
        }
        await page.ClickAsync($"#delete-product-button-{productCount / 2}");
        IElementHandle element = await page.QuerySelectorAsync($"td:has-text(\"Product{productCount / 2}\")");
        Assert.Null(element);
    }
}
