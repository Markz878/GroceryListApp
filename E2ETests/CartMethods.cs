using Microsoft.Playwright;
using System.Globalization;
using System.Threading.Tasks;

namespace E2ETests;

internal static class CartMethods
{
    internal static async Task AddProductToCart(this IPage page, string name, double amount, double price)
    {
        await page.FillAsync("#newproduct-name-input", name);
        await page.FillAsync("#newproduct-amount-input", amount.ToString(CultureInfo.InvariantCulture));
        await page.FillAsync("#newproduct-price-input", price.ToString(CultureInfo.InvariantCulture));
        await page.ClickAsync("button:has-text(\"Add\")");
    }
}
