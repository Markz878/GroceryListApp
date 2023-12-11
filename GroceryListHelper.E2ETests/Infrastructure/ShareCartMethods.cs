using Microsoft.Playwright;

namespace GroceryListHelper.E2ETests.Infrastructure;

internal static class ShareCartMethods
{
    internal static async Task StartShare(IPage page1, IPage page2, string user2Email)
    {
        await page1.GetByLabel("profile-btn").ClickAsync();
        await page1.GetByRole(AriaRole.Link, new() { Name = "Manage Groups" }).ClickAsync();
        foreach (ILocator item in await page1.GetByRole(AriaRole.Button, new() { Name = "Delete group" }).AllAsync())
        {
            await item.ClickAsync();
            await page1.GetByRole(AriaRole.Button, new() { Name = "Yes" }).ClickAsync();
        }
        await page1.GetByRole(AriaRole.Button, new() { Name = "Create group" }).ClickAsync();
        await page1.GetByPlaceholder("Group name").FillAsync("Test Group");
        await page1.GetByPlaceholder("New member email").FillAsync(user2Email);
        await page1.GetByRole(AriaRole.Button, new() { Name = "+" }).ClickAsync();
        await page1.GetByRole(AriaRole.Button, new() { Name = "Create group" }).ClickAsync();
        await page1.GetByLabel("Select group cart").ClickAsync();

        await page2.GetByLabel("profile-btn").ClickAsync();
        await page2.GetByRole(AriaRole.Link, new() { Name = "Manage Groups" }).ClickAsync();
        await page2.GetByLabel("Select group cart").ClickAsync();

        await page1.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();
    }
}
