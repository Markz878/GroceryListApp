using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.Playwright;
using System.IO;
using System.Text.Json;
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

    [Fact]
    public async Task UserCanChangeEmail()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        UserCredentialsModel user = await page.CreateUser();
        await page.ClickAsync("a[href=profile]");
        await page.FillAsync("#new-email", "testnew123@gmail.com");
        await page.FillAsync("#password-for-email-change", user.Password);
        await page.ClickAsync("button:has-text(\"Change email\")");
        await page.WaitForSelectorAsync("a[href=login]");
    }

    [Fact]
    public async Task UserCanChangePassword()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        UserCredentialsModel user = await page.CreateUser();
        string newPassword = user.Password + "!";
        await page.ClickAsync("a[href=profile]");
        await page.FillAsync("#current-password", user.Password);
        await page.FillAsync("#new-password", newPassword);
        await page.FillAsync("#confirm-new-password", newPassword);
        await page.ClickAsync("button:has-text(\"Change password\")");
        await page.WaitForSelectorAsync("a[href=login]");
    }

    [Fact]
    public async Task UserCanLoadPersonalInfo()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        UserCredentialsModel user = await page.CreateUser();
        await page.ClickAsync("a[href=profile]");
        IDownload download = await page.RunAndWaitForDownloadAsync(() => page.ClickAsync("#download-btn"));
        Stream stream = await download.CreateReadStreamAsync();
        EmailModel emailModel = await JsonSerializer.DeserializeAsync<EmailModel>(stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        Assert.Equal(user.Email, emailModel.Email);
        await download.DeleteAsync();
    }

    [Fact]
    public async Task UserCanDeleteProfile()
    {
        await using IBrowserContext BrowserContext = await fixture.BrowserInstance.GetNewBrowserContext();
        IPage page = await BrowserContext.GotoPage();
        UserCredentialsModel user = await page.CreateUser();
        await page.ClickAsync("a[href=profile]");
        await page.FillAsync("#password-for-account-deletion", user.Password);
        await page.ClickAsync("#delete-btn");
        await page.WaitForSelectorAsync("a[href=login]");
    }
}
