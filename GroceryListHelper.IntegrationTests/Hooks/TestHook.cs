using BoDi;
using GroceryListHelper.IntegrationTests.PageObjects;
using Microsoft.Playwright;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace GroceryListHelper.IntegrationTests.Hooks
{
    [Binding]
    public class TestHook
    {
        [BeforeScenario("CanAddCartProduct")]
        public async Task BeforeCanAddCartProductScenario(IObjectContainer container)
        {
            IPlaywright playwright = await Playwright.CreateAsync();
            IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = false,
                SlowMo = 2000
            });
            IndexPageObject pageObject = new(browser);
            container.RegisterInstanceAs(playwright);
            container.RegisterInstanceAs(browser);
            container.RegisterInstanceAs(pageObject);
        }

        [AfterScenario]
        public async Task AfterScenario(IObjectContainer container)
        {
            IBrowser browser = container.Resolve<IBrowser>();
            await browser.CloseAsync();
            IPlaywright playwright = container.Resolve<IPlaywright>();
            playwright.Dispose();
        }
    }
}
