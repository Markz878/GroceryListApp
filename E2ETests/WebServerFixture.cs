using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable once ClassNeverInstantiated.Global
namespace E2ETests
{
    public class WebServerFixture : IAsyncLifetime, IDisposable
    {
        private readonly IHost host;
        private IPlaywright playwright;
        public IBrowserContext BrowserContext { get; private set; }
        public string BaseUrl { get; } = $"http://localhost:5000";

        public WebServerFixture()
        {
            host = GroceryListHelper.Server.Program
                .CreateHostBuilder(null)
                .ConfigureAppConfiguration(c => c.AddUserSecrets("e73b7a0b-29e9-4ce8-ba58-7f7b4393ab50"))
                .Build();
        }

        public async Task InitializeAsync()
        {
            playwright = await Playwright.CreateAsync();
            IBrowser browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = false,
                SlowMo = 5000,
            });
            BrowserContext = await browser.NewContextAsync(new BrowserNewContextOptions()
            {
                IgnoreHTTPSErrors = true,
                BaseURL = "http://localhost:5000",
            });
            await host.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await host.StopAsync();
            host?.Dispose();
            playwright?.Dispose();
        }

        public void Dispose()
        {
            host?.Dispose();
            playwright?.Dispose();
        }
    }
}
