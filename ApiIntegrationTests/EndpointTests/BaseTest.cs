using System.Text.Json;

namespace ApiIntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public abstract class BaseTest : IDisposable
{
    protected readonly WebApplicationFactoryFixture _factory;
    protected readonly HttpClient _client;
    protected static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public BaseTest(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        factory.TestOutputHelper = testOutputHelper;
        _client = factory.CreateClient();
    }

    public CartProductDbModel GetRandomDbCartProduct(int order = 0)
    {
        return new CartProductDbModel()
        {
            Id = Guid.NewGuid(),
            Amount = Random.Shared.NextDouble() * 10,
            Name = "Product" + Random.Shared.Next(0, 10000),
            Order = order,
            UnitPrice = Random.Shared.NextDouble() * 10,
            UserId = TestAuthHandler.UserId,
        };
    }

    public StoreProductDbModel GetRandomDbStoreProduct()
    {
        return new StoreProductDbModel()
        {
            Id = Guid.NewGuid(),
            Name = "Product" + Random.Shared.Next(0, 10000),
            UnitPrice = Random.Shared.NextDouble() * 10,
            UserId = TestAuthHandler.UserId,
        };
    }

    public async Task<List<CartProductDbModel>> SaveCartProducts(int n)
    {
        List<CartProductDbModel> result = new(n);
        for (int i = 0; i < n; i++)
        {
            result.Add(GetRandomDbCartProduct(i * 1000));
        }
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        db.CartProducts.AddRange(result);
        await db.SaveChangesAsync();
        return result;
    }

    public async Task<List<StoreProductDbModel>> SaveStoreProducts(int n)
    {
        List<StoreProductDbModel> result = new(n);
        for (int i = 0; i < n; i++)
        {
            result.Add(GetRandomDbStoreProduct());
        }
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        db.StoreProducts.AddRange(result);
        await db.SaveChangesAsync();
        return result;
    }

    public void Dispose()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        db.CartProducts.RemoveRange(db.CartProducts);
        db.StoreProducts.RemoveRange(db.StoreProducts);
        db.SaveChanges();
        GC.SuppressFinalize(this);
    }
}
