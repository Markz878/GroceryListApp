using GroceryListHelper.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace E2ETests;

public class WebApplicationFactoryFixture : WebApplicationFactory<GroceryListHelper.Server.Program>
{
    public ITestOutputHelper TestOutputHelper { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder hostBuilder)
    {
        hostBuilder.UseUrls("https://localhost:5001");

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

    //public async Task<IPage> GetPlaywrightPage(string path = "")
    //{
    //    PlaywrightInstance ??= await Playwright.CreateAsync();
    //    BrowserInstance ??= await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
    //    {
    //        Headless = false,
    //        SlowMo = 2500,
    //    });
    //    BrowserContext ??= await BrowserInstance.NewContextAsync(new BrowserNewContextOptions()
    //    {
    //        IgnoreHTTPSErrors = true,
    //    });
    //    IPage page = await BrowserContext.NewPageAsync();
    //    await page.GotoAsync("https://localhost:5001" + path);
    //    return page;
    //}

    public override ValueTask DisposeAsync()
    {
        //try
        //{
        //    CosmosClient cosmosClient = new(configuration.GetConnectionString("Cosmos"));
        //    QueryDefinition queryDefinition = new("SELECT * FROM c");
        //    using FeedIterator<DatabaseProperties> feedIterator = cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>(queryDefinition);
        //    while (feedIterator.HasMoreResults)
        //    {
        //        FeedResponse<DatabaseProperties> response = await feedIterator.ReadNextAsync();
        //        foreach (DatabaseProperties database in response)
        //        {
        //            if (database.Id == "TestDb")
        //            {
        //                Database db = cosmosClient.GetDatabase(database.Id);
        //                await db.DeleteAsync();
        //            }
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.Message);
        //}
        using IServiceScope scope = Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        db.Database.EnsureDeleted();
        GC.SuppressFinalize(this);
        return base.DisposeAsync();
    }
}
