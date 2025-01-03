using GroceryListHelper.Core.Features.Users;

namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

public sealed class WebApplicationFactoryFixture : WebApplicationFactory<Server.Program>, IAsyncLifetime
{
    public required ITestOutputHelper TestOutputHelper { get; set; }

    public async Task InitializeAsync()
    {
        using IServiceScope scope = Server.Services.CreateScope();
        await scope.ServiceProvider.SeedDatabase();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new AddUserCommand() { Email = TestAuthHandler.UserEmail, Name = "Test User" });
        await mediator.Send(new AddUserCommand() { Email = TestAuthHandler.RandomEmail1, Name = "Random User 1" });
        await mediator.Send(new AddUserCommand() { Email = TestAuthHandler.RandomEmail2, Name = "Random User 2" });
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
