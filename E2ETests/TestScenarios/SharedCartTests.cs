using E2ETests.Infrastructure;
using GroceryListHelper.DataAccess.Repositories;
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


    public async Task InitializeAsync()
    {
        using IServiceScope scope = server.Services.CreateScope();
        IUserRepository db = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        await db.AddUser(fakeAuth1.Email, fakeAuth1.Guid, fakeAuth1.UserName);
        await db.AddUser(fakeAuth2.Email, fakeAuth2.Guid, fakeAuth2.UserName);
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
    }

    [Fact]
    public async Task CartSharing_Success()
    {
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await page1.AddProductToCart(productName, productAmount, productPrice);
        IElementHandle? element = await page2.QuerySelectorAsync("#item-name-0");
        ArgumentNullException.ThrowIfNull(element);
        string elementText = await element.InnerTextAsync();
        Assert.Equal(productName, elementText);

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
        Assert.Equal("3.10", priceText);

        await page2.ClickAsync("#delete-product-button-0");
        IElementHandle? deletedElement = await page1.QuerySelectorAsync("#item-name-0");
        Assert.Null(deletedElement);

        await page2.GetByRole(AriaRole.Button, new() { Name = "Stop sharing" }).ClickAsync();
        await Task.Delay(500);
        IElementHandle? messageElement = await page1.GetByText($"{fakeAuth2.Email} has left sharing.").ElementHandleAsync();
        Assert.NotNull(messageElement);
    }
}
