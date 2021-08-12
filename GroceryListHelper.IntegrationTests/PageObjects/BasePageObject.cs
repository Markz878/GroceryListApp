using Microsoft.Playwright;
using System.Threading.Tasks;

namespace GroceryListHelper.IntegrationTests.PageObjects
{
    public abstract class BasePageObject
    {
        public abstract string PagePath { get; }
        public IPage Page { get; set; }
        public IBrowser Browser { get; set; }

        public BasePageObject(IBrowser browser)
        {
            Browser = browser;
        }

        public async Task NavigateAsync()
        {
            Page = await Browser.NewPageAsync();
            await Page.GotoAsync(PagePath);
        }
    }
}
