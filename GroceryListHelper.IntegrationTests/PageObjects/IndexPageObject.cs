using Microsoft.Playwright;

namespace GroceryListHelper.IntegrationTests.PageObjects
{
    public class IndexPageObject : BasePageObject
    {
        public override string PagePath => "https://localhost:5001";

        public IndexPageObject(IBrowser browser) : base(browser)
        {
        }
    }
}
