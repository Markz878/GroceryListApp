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

    public SharedCartTests(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
    {
        server.CreateDefaultClient();
        server.TestOutputHelper = testOutputHelper;
        this.server = server;
    }

    [Fact]
    public async Task AddValidProductToCart()
    {
        await using IBrowserContext BrowserContext1 = await server.GetNewBrowserContext(fakeAuth1);
        IPage page1 = await BrowserContext1.GotoPage(server.BaseUrl, true);
        await using IBrowserContext BrowserContext2 = await server.GetNewBrowserContext(fakeAuth2);
        IPage page2 = await BrowserContext2.GotoPage(server.BaseUrl, true);
        string productName = "Maito";
        int productAmount = 2;
        double productPrice = 2.9;
        await ShareCartMethods.StartShare(page1, page2, fakeAuth1.Email, fakeAuth2.Email);
        await page1.AddProductToCart(productName, productAmount, productPrice);
        await Task.Delay(1000);
        IElementHandle? element = await page2.QuerySelectorAsync("#item-name-0");
        ArgumentNullException.ThrowIfNull(element);
        string? page2Text = await page2.TextContentAsync("#content");
        Assert.NotNull(element);
    }


    public async Task InitializeAsync()
    {
        using IServiceScope scope = server.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
    public async Task DisposeAsync()
    {
        using IServiceScope scope = server.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        await db.Database.EnsureDeletedAsync();
    }
}
