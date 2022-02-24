using GroceryListHelper.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using System.Net;
using System.Net.Sockets;
using Xunit;
using Xunit.Abstractions;

namespace E2ETests;

public class AuthorizedWebApplicationFactoryFixture : BaseWebApplicationFactoryFixture
{
    public AuthorizedWebApplicationFactoryFixture()
    {
        AddFakeAuthentication = true;
    }
}

public class WebApplicationFactoryFixture : BaseWebApplicationFactoryFixture
{
    public WebApplicationFactoryFixture()
    {

    }
}

public abstract class BaseWebApplicationFactoryFixture : WebApplicationFactory<GroceryListHelper.Server.Program>, IAsyncLifetime
{
    public ITestOutputHelper TestOutputHelper { get; set; }
    public IPlaywright PlaywrightInstance { get; set; }
    public IBrowser BrowserInstance { get; set; }
    public string BaseUrl { get; } = $"https://localhost:{GetRandomUnusedPort()}";
    public bool AddFakeAuthentication { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder hostBuilder)
    {
        hostBuilder.UseUrls(BaseUrl);

        hostBuilder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "IpRateLimiting:EnableEndpointRateLimiting", "false" },
                { "Logging:LogLevel:Microsoft.EntityFrameworkCore", "Warning" }
            });
        });

        hostBuilder.ConfigureServices((ctx, services) =>
        {
            ServiceDescriptor descriptor = services.SingleOrDefault(d => d.ServiceType ==
                typeof(DbContextOptions<GroceryStoreDbContext>));

            services.Remove(descriptor);

            services.AddDbContext<GroceryStoreDbContext>(options =>
            {
                options.UseCosmos(ctx.Configuration.GetConnectionString("Cosmos"), $"TestDb");
            });

            if (AddFakeAuthentication)
            {
                services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            }

            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
            db.Database.EnsureCreated();
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
            //Headless = false,
            //SlowMo = 200,
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await BrowserInstance.DisposeAsync();
        PlaywrightInstance.Dispose();
        using IServiceScope scope = Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        db.Database.EnsureDeleted();
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
