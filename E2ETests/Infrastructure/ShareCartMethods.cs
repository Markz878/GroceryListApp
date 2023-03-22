using Microsoft.Playwright;

namespace E2ETests.Infrastructure;

internal static class ShareCartMethods
{
    internal static async Task StartShare(IPage page1, IPage page2, string email1, string email2)
    {
        await page1.GetByRole(AriaRole.Button, new() { Name = "Cart Groups" }).ClickAsync();
        foreach (ILocator item in await page1.GetByRole(AriaRole.Button, new() { Name = "×" }).AllAsync())
        {
            await item.ClickAsync();
            await page1.GetByRole(AriaRole.Button, new() { Name = "Ok" }).ClickAsync();
        }
        await page1.GetByRole(AriaRole.Button, new() { Name = "Create group" }).ClickAsync();
        await page1.GetByPlaceholder("Group name").FillAsync("Test Group");
        await page1.GetByPlaceholder("New member email").FillAsync(email2);
        await page1.GetByRole(AriaRole.Button, new() { Name = "+" }).ClickAsync();
        await page1.GetByRole(AriaRole.Button, new() { Name = "Create group" }).ClickAsync();
        await page1.GetByRole(AriaRole.Heading, new() { Name = "Test Group" }).ClickAsync();
        await page1.GetByRole(AriaRole.Button, new() { Name = "Join cart sharing" }).ClickAsync();

        await page2.GetByRole(AriaRole.Button, new() { Name = "Cart Groups" }).ClickAsync();
        await page2.GetByRole(AriaRole.Heading, new() { Name = "Test Group" }).ClickAsync();
        await page2.GetByRole(AriaRole.Button, new() { Name = "Join cart sharing" }).ClickAsync();
    }
}
