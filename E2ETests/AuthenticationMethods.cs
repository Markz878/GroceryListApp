using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace E2ETests;

internal static class AuthenticationMethods
{
    internal static async Task<UserCredentialsModel> CreateUser(this IPage page)
    {
        string email = $"test{Random.Shared.Next(10000)}@gmail.com";
        string password = "Habla51";
        await page.ClickAsync("a:has-text(\"Register\")");
        await page.FillAsync("#email", email);
        await page.FillAsync("#password", password);
        await page.FillAsync("#confirm-password", password);
        await page.ClickAsync("button:has-text(\"Register\")");
        await page.ClickAsync("h2:has-text(\"Grocery List Helper\")");
        return new UserCredentialsModel { Email = email, Password = password };
    }
}
