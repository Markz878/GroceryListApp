﻿using GroceryListHelper.DataAccess;
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

namespace E2ETests.Infrastructure;

public sealed class WebApplicationFactoryFixture : WebApplicationFactory<GroceryListHelper.Server.Program>, IAsyncLifetime
{
    public ITestOutputHelper? TestOutputHelper { get; set; }
    public IPlaywright? PlaywrightInstance { get; private set; }
    public IBrowser? BrowserInstance { get; private set; }
    public string BaseUrl { get; } = $"https://localhost:{GetRandomUnusedPort()}";

    protected override void ConfigureWebHost(IWebHostBuilder hostBuilder)
    {
        hostBuilder.UseUrls(BaseUrl);

        hostBuilder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>()
            {
                { "Logging:LogLevel:Microsoft.EntityFrameworkCore", "Warning" }
            });
        });

        hostBuilder.ConfigureServices((ctx, services) =>
        {
            services.RemoveAll<DbContextOptions<GroceryStoreDbContext>>();
            services.AddDbContext<GroceryStoreDbContext>(options =>
            {
                options.UseCosmos(ctx.Configuration.GetConnectionString("Cosmos") ?? throw new ArgumentNullException("Cosmos connection string"), "E2ETestDb");
            });
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
            Headless = true,
            SlowMo = 200,
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (BrowserInstance is not null && PlaywrightInstance is not null)
        {
            await BrowserInstance.DisposeAsync();
            PlaywrightInstance.Dispose();
        }
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