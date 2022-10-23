using Microsoft.Playwright;
using System.Globalization;

namespace E2ETests;

internal static class PlaywrightHelpers
{
    internal static async Task<IBrowserContext> GetNewBrowserContext(this IBrowser browser)
    {
        return await browser.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true,
        });
    }

    internal static async Task<IPage> GotoPage(this IBrowserContext browserContext, string url, bool checkIfAuthenticated = false)
    {
        IPage page = await browserContext.NewPageAsync();
        await page.GotoAsync(url);
        if (checkIfAuthenticated)
        {
            await page.Locator("#signout-btn").WaitForAsync();
        }
        await Task.Delay(1000);
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