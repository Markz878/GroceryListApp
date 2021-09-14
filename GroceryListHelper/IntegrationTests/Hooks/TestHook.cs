using BoDi;
using GroceryListHelper.IntegrationTests.PageObjects;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace GroceryListHelper.IntegrationTests.Hooks
{
    [Binding]
    public class TestHook
    {
        private const string apiContainerName = "grocerylisthelperserver";

        [BeforeTestRun]
        public static void StartSiteInDocker()
        {
            ExecuteCommand($"docker run -d -p 5000:80 --name {apiContainerName} {apiContainerName} -e AccessTokenKey='qwertyuiop1234567890' -e RefreshTokenKey='qwertyuiop1234567890'");
        }

        private static void ExecuteCommand(string command)
        {
            Process process = Process.Start("cmd.exe", "/C " + command);
            process.WaitForExit();
            process.Dispose();
        }

        [BeforeScenario("AnonymousUserCartInteraction")]
        public async Task BeforeCanAddCartProductScenario(IObjectContainer container)
        {
            IPlaywright playwright = await Playwright.CreateAsync();
            IBrowser browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                //Headless = false,
                //SlowMo = 500,
            });
            IBrowserContext context = await browser.NewContextAsync(new BrowserNewContextOptions()
            {
                IgnoreHTTPSErrors = true,
                BaseURL = "http://localhost:5000",
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

        [AfterTestRun]
        public static void ShutdownServer()
        {
            ExecuteCommand($"docker stop {apiContainerName}");
            ExecuteCommand($"docker rm {apiContainerName}");
        }
    }
}
