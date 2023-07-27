using GroceryListHelper.Core.RepositoryContracts;
using Microsoft.Playwright;

namespace E2ETests.Infrastructure;

internal static class ShareCartMethods
{
    public static FakeAuthInfo FakeAuth1 { get; } = new("Test User 1", "test_user1@email.com", Guid.NewGuid());
    public static FakeAuthInfo FakeAuth2 { get; } = new("Test User 2", "test_user2@email.com", Guid.NewGuid());

    internal static async Task AddFakeUsers(IUserRepository db)
    {
        await db.AddUser(FakeAuth1.Email, FakeAuth1.Guid, FakeAuth1.UserName);
        await db.AddUser(FakeAuth2.Email, FakeAuth2.Guid, FakeAuth2.UserName);
    }

    internal static async Task StartShare(IPage page1, IPage page2, string user2Email)
    {
        await page1.GetByRole(AriaRole.Button, new() { Name = "TU" }).ClickAsync();
        await page1.GetByRole(AriaRole.Link, new() { Name = "Manage Groups" }).ClickAsync();
        foreach (ILocator item in await page1.GetByRole(AriaRole.Button, new() { Name = "Delete group" }).AllAsync())
        {
            await item.ClickAsync();
            await page1.GetByRole(AriaRole.Button, new() { Name = "Ok" }).ClickAsync();
        }
        await page1.GetByRole(AriaRole.Button, new() { Name = "Create group" }).ClickAsync();
        await page1.GetByPlaceholder("Group name").FillAsync("Test Group");
        await page1.GetByPlaceholder("New member email").FillAsync(user2Email);
        await page1.GetByRole(AriaRole.Button, new() { Name = "+" }).ClickAsync();
        await page1.GetByRole(AriaRole.Button, new() { Name = "Create group" }).ClickAsync();
        await page1.GetByRole(AriaRole.Button, new() { Name = "Select group cart" }).ClickAsync();
        await page1.GetByRole(AriaRole.Button, new() { Name = "Join cart sharing" }).ClickAsync();

        await page2.GetByRole(AriaRole.Button, new() { Name = "TU" }).ClickAsync();
        await page2.GetByRole(AriaRole.Link, new() { Name = "Manage Groups" }).ClickAsync();
        await page2.GetByRole(AriaRole.Button, new() { Name = "Select group cart" }).ClickAsync();
        await page2.GetByRole(AriaRole.Button, new() { Name = "Join cart sharing" }).ClickAsync();
    }
}
