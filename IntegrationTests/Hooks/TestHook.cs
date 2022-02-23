using BoDi;
using GroceryListHelper.IntegrationTests.PageObjects;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace GroceryListHelper.IntegrationTests.Hooks;

[Binding]
public class TestHook
{
    //private const string apiContainerName = "grocerylisthelper";
    private static readonly WebApplicationFactoryFixture webApplicationFactoryFixture = new WebApplicationFactoryFixture();

    [BeforeTestRun]
    public static async Task StartSiteInDocker()
    {
        //ProcessStartInfo processStartInfo = new("cmd.exe")
        //{
        //    RedirectStandardInput = true,
        //    RedirectStandardOutput = true,
        //    UseShellExecute = false,
        //};
        //using Process process = Process.Start(processStartInfo);
        //await process.StandardInput.WriteLineAsync(@$"docker build -t {apiContainerName} C:\Users\marku\source\repos\GroceryListHelper");
        //await process.StandardInput.WriteLineAsync($"docker run -p 5000:80 --rm --name {apiContainerName} {apiContainerName} -e AccessTokenKey='qwertyuiop1234567890' -e RefreshTokenKey='qwertyuiop1234567890'");
        //while (true)
        //{
        //    char[] buffer = new char[4096];
        //    await process.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
        //    string result = new(buffer);
        //    if (result.Contains("docker run"))
        //    {
        //        break;
        //    }
        //}
        webApplicationFactoryFixture.CreateDefaultClient();
        await Task.Delay(3000);
        //ExecuteCommand($"docker build -d -p 5000:80 --name {apiContainerName} {apiContainerName} '");
        //ExecuteCommand($"docker run -d -p 5000:80 --name {apiContainerName} {apiContainerName} -e AccessTokenKey='qwertyuiop1234567890' -e RefreshTokenKey='qwertyuiop1234567890'");
    }

    //private static void ExecuteCommand(string command)
    //{
    //    process.WaitForExit();
    //    process.Dispose();
    //}

    [BeforeScenario("AnonymousUserCartInteraction")]
    public async Task BeforeCanAddCartProductScenario(IObjectContainer container)
    {
        IPlaywright playwright = await Playwright.CreateAsync();
        IBrowser browser = await playwright.Firefox.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 200,
        });
        IBrowserContext context = await browser.NewContextAsync(new()
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

    [AfterTestRun]
    public static async Task ShutdownServer()
    {
        //ProcessStartInfo processStartInfo = new("cmd.exe")
        //{
        //    RedirectStandardInput = true,
        //    RedirectStandardOutput = true,
        //    UseShellExecute = false,
        //};
        //using Process process = Process.Start(processStartInfo);
        //await process.StandardInput.WriteLineAsync($"docker stop {apiContainerName}");
        //while (true)
        //{
        //    char[] buffer = new char[4096];
        //    await process.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
        //    string result = new(buffer);
        //    if (result.Contains("docker stop"))
        //    {
        //        break;
        //    }
        //}
        //await Task.Delay(1000);
        await webApplicationFactoryFixture.DisposeAsync();
    }
}
