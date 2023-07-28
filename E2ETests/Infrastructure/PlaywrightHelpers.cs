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
            ColorScheme = ColorScheme.Dark
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

    internal static ILocator GetRow(this IPage page, int index)
    {
        return page.GetByRole(AriaRole.Rowgroup).GetByRole(AriaRole.Row).Nth(index);
    }

    internal static async Task ClickReorderButton(this IPage page, int index)
    {
        await page.GetRow(index).GetByLabel("Reorder").ClickAsync();
    }

    internal static async Task ClickRowCollected(this IPage page, int index)
    {
        await page.GetRow(index).GetByLabel("Product collected").ClickAsync();
    }

    internal static async Task<string> GetItemName(this IPage page, int index)
    {
        return await page.GetRow(index).GetByLabel("Product name").InnerTextAsync();
    }

    internal static async Task<string> GetItemAmount(this IPage page, int index)
    {
        return await page.GetRow(index).GetByLabel("Amount").InnerTextAsync();
    }

    internal static async Task<string> GetItemPrice(this IPage page, int index)
    {
        return await page.GetRow(index).GetByLabel("Unit price").InnerTextAsync();
    }

    internal static async Task<string> GetTotalPrice(this IPage page, int index)
    {
        return await page.GetRow(index).GetByLabel("Total price").InnerTextAsync();
    }

    internal static async Task ClickEditButton(this IPage page, int index)
    {
        await page.GetRow(index).GetByLabel("Edit product").ClickAsync();
    }

    internal static async Task ClickDeleteButton(this IPage page, int index)
    {
        await page.GetRow(index).GetByLabel("Delete product").ClickAsync();
    }

    internal static async Task FillEditAmount(this IPage page, int index, double amount)
    {
        await page.GetRow(index).GetByLabel("Edit amount").FillAsync(amount.ToString(CultureInfo.InvariantCulture));
    }

    internal static async Task FillEditPrice(this IPage page, int index, double amount)
    {
        await page.GetRow(index).GetByLabel("Edit unit price").FillAsync(amount.ToString(CultureInfo.InvariantCulture));
    }

    internal static async Task ClickSubmitEditButton(this IPage page, int index)
    {
        await page.GetRow(index).GetByLabel("Submit edit").ClickAsync();
    }

    internal static async Task<int> GetRowTopPixels(this IPage page, int index)
    {
        string top = await page.GetRow(index).EvaluateAsync<string>(@"element => window.getComputedStyle(element).getPropertyValue('top')");
        int pixels = int.Parse(top.Replace("px", ""));
        return pixels;
    }
}