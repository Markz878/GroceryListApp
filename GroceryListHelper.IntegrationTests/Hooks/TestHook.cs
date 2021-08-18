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
        [BeforeScenario("AnonymousUserCartInteraction")]
        public async Task BeforeCanAddCartProductScenario(IObjectContainer container)
        {
            IPlaywright playwright = await Playwright.CreateAsync();
            IBrowser browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = false,
                SlowMo = 1000,
            });
            IBrowserContext context = await browser.NewContextAsync(new BrowserNewContextOptions()
            {
                IgnoreHTTPSErrors = true,
                BaseURL = "https://localhost:5001",
            });
            IndexPageObject indexPage = new(context);
            LoginPageObject loginPage = new(context);
            container.RegisterInstanceAs(playwright);
            container.RegisterInstanceAs(browser);
            container.RegisterInstanceAs(indexPage);
            container.RegisterInstanceAs(loginPage);
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
