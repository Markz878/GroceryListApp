using Microsoft.Playwright;

namespace GroceryListHelper.IntegrationTests.PageObjects;

public sealed class LoginPageObject : BasePageObject
{
    public LoginPageObject(IBrowserContext browser) : base(browser, "/login")
    {
    }
}
