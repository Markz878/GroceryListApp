using Microsoft.Playwright;
using System.Threading.Tasks;

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

    internal static async Task<IPage> GotoPage(this IBrowserContext browserContext, string url)
    {
        IPage page = await browserContext.NewPageAsync();
        await page.GotoAsync(url);
        return page;
    }
}