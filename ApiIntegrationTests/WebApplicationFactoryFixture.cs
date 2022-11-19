using GroceryListHelper.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit.Abstractions;

namespace ApiIntegrationTests;

public class WebApplicationFactoryFixture : WebApplicationFactory<GroceryListHelper.Server.Program>, IAsyncLifetime
{
    public required ITestOutputHelper TestOutputHelper { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((ctx, services) =>
        {
            services.RemoveAll<DbContextOptions<GroceryStoreDbContext>>();
            services.AddDbContext<GroceryStoreDbContext>(options =>
            {
                options.UseCosmos(ctx.Configuration.GetConnectionString("Cosmos") ?? throw new ArgumentNullException("Connection string"), "TestDb");
            });
            services.AddAuthentication("FakeAuth").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("FakeAuth", null);
        });
    }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        await db.Database.EnsureDeletedAsync();
    }
}
