namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

public sealed class WebApplicationFactoryFixture : WebApplicationFactory<Server.Program>
{
    public required ITestOutputHelper TestOutputHelper { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((ctx, services) =>
        {
            services.AddAuthentication("FakeAuth").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("FakeAuth", null);
        });
    }
}
