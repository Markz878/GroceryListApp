using Microsoft.Playwright;

namespace GroceryListHelper.IntegrationTests.PageObjects;

public sealed class IndexPageObject : BasePageObject
{
    public IndexPageObject(IBrowserContext browser) : base(browser, "")
    {
    }
}
