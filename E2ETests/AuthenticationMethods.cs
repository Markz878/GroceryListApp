using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace E2ETests;

internal static class AuthenticationMethods
{
    internal static async Task<string> CreateUser(this IPage page)
    {
        string email = $"test{Random.Shared.Next(10000)}@gmail.com";
        await page.ClickAsync("a:has-text(\"Register\")");
        await page.FillAsync("#email", email);
        await page.FillAsync("#password", "Hablahattu51");
        await page.FillAsync("#confirm-password", "Hablahattu51");
        await page.ClickAsync("button:has-text(\"Register\")");
        await page.ClickAsync("h2:has-text(\"Grocery List Helper\")");
        return email;
    }
}
