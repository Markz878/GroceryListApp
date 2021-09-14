using Microsoft.Playwright;
using System.Threading.Tasks;

namespace GroceryListHelper.IntegrationTests.PageObjects
{
    public abstract class BasePageObject
    {
        public string PagePath { get; }
        public IPage Page { get; set; }
        public IBrowserContext Browser { get; set; }

        public BasePageObject(IBrowserContext browser, string path)
        {
            Browser = browser;
            PagePath = path;
        }

        public async Task NavigateAsync()
        {
            Page = await Browser.NewPageAsync();
            await Page.GotoAsync(PagePath);
        }
    }
}
