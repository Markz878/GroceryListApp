using GroceryListHelper.DataAccess;
using GroceryListHelper.DataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ApiIntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public abstract class BaseTest : IDisposable
{
    protected readonly WebApplicationFactoryFixture _factory;
    protected readonly HttpClient _client;
    protected readonly IServiceScope _scope;

    public BaseTest(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        factory.TestOutputHelper = testOutputHelper;
        _client = factory.CreateClient();
        _scope = _factory.Services.CreateScope();
    }

    protected GroceryStoreDbContext GetDbContext() => _scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();

    public static IEnumerable<CartProductDbModel> GenerateCartProducts(int n)
    {
        for (int i = 0; i < n; i++)
        {
            yield return new CartProductDbModel()
            {
                Id = Guid.NewGuid(),
                Amount = Random.Shared.NextDouble() * 10,
                Name = "Product" + Random.Shared.Next(0, 10000),
                Order = i * 1000,
                UnitPrice = Random.Shared.NextDouble() * 10,
                UserId = TestAuthHandler.UserId,
            };
        }
    }

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);
    }
}
