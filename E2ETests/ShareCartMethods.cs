using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace E2ETests;

internal static class ShareCartMethods
{
    internal static async Task StartShare(IPage page1, IPage page2)
    {
        UserCredentialsModel user1 = await page1.CreateUser();
        UserCredentialsModel user2 = await page2.CreateUser();
        await page1.FillAsync("#share-cart-email-input", user2.Email);
        await page1.ClickAsync("#add-user-email-btn");
        await page1.ClickAsync("#share-cart-btn");
        await page2.ClickAsync("#select-join-cart-btn");
        await page2.FillAsync("#cart-owner-email-input", user1.Email);
        await page2.ClickAsync("#join-cart-btn");
    }
}
