using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests.TestScenarios;

[Collection(nameof(WebApplicationFactoryCollection))]
public class ProfilePageTests
{
    private readonly WebApplicationFactoryFixture fixture;

    public ProfilePageTests(WebApplicationFactoryFixture server, ITestOutputHelper testOutputHelper)
    {
        server.CreateDefaultClient();
        server.TestOutputHelper = testOutputHelper;
        fixture = server;
    }

    [Fact]
    public async Task UserCanRegister()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        await page.CreateUser();
    }
}
