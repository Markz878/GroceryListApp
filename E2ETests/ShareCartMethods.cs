using Microsoft.Playwright;

namespace E2ETests;

internal static class ShareCartMethods
{
    internal static async Task StartShare(IPage page1, IPage page2)
    {
        await page1.FillAsync("#share-cart-email-input", "test123@gmail.com");
        await page1.ClickAsync("#add-user-email-btn");
        await page1.ClickAsync("#share-cart-btn");
        await Task.Delay(1000);
        await page2.ClickAsync("#select-join-cart-btn");
        await page2.FillAsync("#cart-owner-email-input", "test456@gmail.com");
        await page2.ClickAsync("#join-cart-btn");
    }
}
