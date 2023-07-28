using E2ETests.Infrastructure;
using GroceryListHelper.Core.RepositoryContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests.TestScenarios;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class SharedCartTests : IAsyncLifetime
{
    private readonly WebApplicationFactoryFixture server;
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
        await ShareCartMethods.AddFakeUsers(db);
        browserContext1 = await server.GetNewBrowserContext(ShareCartMethods.FakeAuth1);
        page1 = await browserContext1.GotoPage(server.BaseUrl, true);
        browserContext2 = await server.GetNewBrowserContext(ShareCartMethods.FakeAuth2);
        page2 = await browserContext2.GotoPage(server.BaseUrl, true);
        await ShareCartMethods.StartShare(page1, page2, ShareCartMethods.FakeAuth2.Email);
    }
    public async Task DisposeAsync()
    {
        await browserContext1.DisposeAsync();
        await browserContext2.DisposeAsync();
    }

    [Fact]
    public async Task CartSharing_Success()
    {
        string productName1 = "Maito";
        string productName2 = "Leipä";
        string productName3 = "Juusto";
        int product1Amount = 2;
        await page1.AddProductToCart(productName1, product1Amount, 2.9);
        await page1.AddProductToCart(productName2, 2, 1.5);
        await page1.AddProductToCart(productName3, 1, 4.5);

        string firstProductName = await page2.GetItemName(0);
        Assert.Equal(productName1, firstProductName);

        await page2.ClickEditButton(0);
        await page2.FillEditAmount(0, product1Amount + 1);
        await page2.FillEditPrice(0, 3.1);
        await page2.ClickSubmitEditButton(0);
        string amountText = await page1.GetItemAmount(0);
        Assert.Equal("3", amountText);
        string priceText = await page1.GetItemPrice(0);
        Assert.Equal("3.10", priceText);

        await page2.ClickDeleteButton(0);
        string currentTopItemName = await page1.GetItemName(0);
        Assert.Equal(productName2, currentTopItemName);

        await page2.GetByRole(AriaRole.Button, new() { Name = "Stop sharing" }).ClickAsync();
        await Task.Delay(500);
        IElementHandle? messageElement = await page1.GetByText($"{ShareCartMethods.FakeAuth2.Email} has left sharing.").ElementHandleAsync();
        Assert.NotNull(messageElement);
    }
}
