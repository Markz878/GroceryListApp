using GroceryListHelper.DataAccess.HelperMethods;

namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

public sealed class WebApplicationFactoryFixture : WebApplicationFactory<Server.Program>, IAsyncLifetime
{
    public required ITestOutputHelper TestOutputHelper { get; set; }

    public Task InitializeAsync()
    {
        using IServiceScope scope = Server.Services.CreateScope();
        scope.ServiceProvider.DeleteDatabase();
        scope.ServiceProvider.InitDatabase();
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((ctx, services) =>
        {
            services.AddAuthentication("FakeAuth").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("FakeAuth", null);
        });
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
