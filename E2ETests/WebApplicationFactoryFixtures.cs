using GroceryListHelper.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using System.Net;
using System.Net.Sockets;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests;

public class SharedWebApplicationFactoryFixture : IAsyncLifetime
{
    public AuthorizedWebApplicationFactoryFixture Server1 { get; } = new AuthorizedWebApplicationFactoryFixture();
    public AuthorizedWebApplicationFactoryFixture2 Server2 { get; } = new AuthorizedWebApplicationFactoryFixture2();

    public async Task InitializeAsync()
    {
        await Server1.InitializeAsync();
        await Server2.InitializeAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await Server1.DisposeAsync();
        await Server2.DisposeAsync();
    }
}

public sealed class AuthorizedWebApplicationFactoryFixture2 : BaseWebApplicationFactoryFixture
{
    public AuthorizedWebApplicationFactoryFixture2() : base(2)
    {
    }
}

public sealed class AuthorizedWebApplicationFactoryFixture : BaseWebApplicationFactoryFixture
{
    public AuthorizedWebApplicationFactoryFixture() : base(1)
    {
    }
}

public sealed class WebApplicationFactoryFixture : BaseWebApplicationFactoryFixture
{
    public WebApplicationFactoryFixture() : base(0)
    {
    }
}

public abstract class BaseWebApplicationFactoryFixture : WebApplicationFactory<GroceryListHelper.Server.Program>, IAsyncLifetime
{
    public ITestOutputHelper TestOutputHelper { get; set; }
    public IPlaywright PlaywrightInstance { get; private set; }
    public IBrowser BrowserInstance { get; private set; }
    public string BaseUrl { get; } = $"https://localhost:{GetRandomUnusedPort()}";

    private readonly int userId;

    public BaseWebApplicationFactoryFixture(int userId)
    {
        this.userId = userId;
    }

    protected override void ConfigureWebHost(IWebHostBuilder hostBuilder)
    {
        hostBuilder.UseUrls(BaseUrl);

        hostBuilder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "Logging:LogLevel:Microsoft.EntityFrameworkCore", "Warning" }
            });
        });

        hostBuilder.ConfigureServices((ctx, services) =>
        {
            services.RemoveAll<DbContextOptions<GroceryStoreDbContext>>();
            services.AddDbContext<GroceryStoreDbContext>(options =>
            {
                options.UseCosmos(ctx.Configuration.GetConnectionString("Cosmos"), "E2ETestDb");
            });
            if (userId == 1)
            {
                services.AddAuthentication("FakeAuth").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("FakeAuth", null);
            }
            else if(userId == 2)
            {
                services.AddAuthentication("FakeAuth").AddScheme<AuthenticationSchemeOptions, TestAuthHandler2>("FakeAuth", null);
            }
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost fixtureHost = builder.Build();
        builder.ConfigureWebHost(b => b.UseKestrel());
        IHost host = builder.Build();
        host.Start();
        return fixtureHost;
    }

    public async Task InitializeAsync()
    {
        PlaywrightInstance = await Playwright.CreateAsync();
        BrowserInstance = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 100,
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await BrowserInstance.DisposeAsync();
        PlaywrightInstance.Dispose();
    }

    private static int GetRandomUnusedPort()
    {
        TcpListener listener = new(IPAddress.Any, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
