using GroceryListHelper.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace E2ETests;

public class WebServerFixture : IAsyncLifetime, IDisposable
{
    private readonly IHost host;
    public IPlaywright playwright { get; private set; }
    public IBrowser browser { get; private set; }
    public IBrowserContext BrowserContext { get; private set; }
    public string BaseUrl { get; } = $"https://localhost:{GetRandomUnusedPort()}";

    public WebServerFixture()
    {
        host = GroceryListHelper.Server.Program
            .CreateHostBuilder(null)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                    //webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(BaseUrl);
            })
            .ConfigureAppConfiguration(c =>
            {
                Dictionary<string, string> inMemoryConfiguration = new Dictionary<string, string>
                {
                        { "ConnectionStrings:DatabaseConnection", "Data Source=database.db;" },
                        { "RefreshTokenKey", "qwertyuiopasdfghjkl" },
                        { "AccessTokenKey", "qwertyuiopasdfghjkl" }
                };
                c.AddInMemoryCollection(inMemoryConfiguration);
            })
            .Build();
    }

    public async Task InitializeAsync()
    {
        playwright = await Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = false,
            SlowMo = 5000,
        });
        BrowserContext = await browser.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true,
            BaseURL = BaseUrl,
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

    private static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

}
