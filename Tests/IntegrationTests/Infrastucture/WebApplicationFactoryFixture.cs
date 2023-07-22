using GroceryListHelper.Core.RepositoryContracts;
using GroceryListHelper.DataAccess.HelperMethods;

namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

public sealed class WebApplicationFactoryFixture : WebApplicationFactory<Server.Program>, IAsyncLifetime
{
    public required ITestOutputHelper TestOutputHelper { get; set; }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = Server.Services.CreateScope();
        scope.ServiceProvider.DeleteDatabase();
        scope.ServiceProvider.InitDatabase();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        await userRepository.AddUser(TestAuthHandler.UserEmail, Guid.NewGuid(), null);
        await userRepository.AddUser(TestAuthHandler.RandomEmail1, Guid.NewGuid(), null);
        await userRepository.AddUser(TestAuthHandler.RandomEmail2, Guid.NewGuid(), null);
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
