using Microsoft.Playwright;
using System.Globalization;

namespace E2ETests.Infrastructure;

internal static class PlaywrightHelpers
{
    internal static async Task<IBrowserContext> GetNewBrowserContext(this WebApplicationFactoryFixture factory, FakeAuthInfo? fakeAuth = null)
    {
        ArgumentNullException.ThrowIfNull(factory.BrowserInstance);
        IBrowserContext browserContext = await factory.BrowserInstance.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true,
        });
        if (fakeAuth != null)
        {
            await browserContext.SetExtraHTTPHeadersAsync(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("fake-username", fakeAuth.UserName),
                new KeyValuePair<string, string>("fake-email", fakeAuth.Email),
                new KeyValuePair<string, string>("fake-userid", fakeAuth.Guid.ToString())
            });
        }
        return browserContext;
    }

    internal static async Task<IPage> GotoPage(this IBrowserContext browserContext, string url, bool checkIfAuthenticated = false)
    {
        IPage page = await browserContext.NewPageAsync();
        await page.GotoAsync(url, new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle });
        if (checkIfAuthenticated)
        {
            await page.Locator("#profile-btn").WaitForAsync();
        }
        return page;
    }

    internal static async Task AddProductToCart(this IPage page, string name, double amount, double price)
    {
        await page.FillAsync("#newproduct-name-input", name);
        await page.FillAsync("#newproduct-amount-input", amount.ToString(CultureInfo.InvariantCulture));
        await page.FillAsync("#newproduct-price-input", price.ToString(CultureInfo.InvariantCulture));
        await page.ClickAsync("button:has-text(\"Add\")");
    }
}