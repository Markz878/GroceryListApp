using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared.Models.HelperModels;
using GroceryListHelper.Shared.Models.StoreProducts;
using GroceryListHelper.Tests.IntegrationTests.Infrastucture;
using System.Text.Json;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public abstract class BaseTest : IDisposable
{
    protected readonly WebApplicationFactoryFixture _factory;
    protected readonly HttpClient _client;
    protected static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    protected readonly IServiceScope _scope;

    protected BaseTest(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        factory.TestOutputHelper = testOutputHelper;
        _client = factory.CreateClient();
        _scope = _factory.Services.CreateScope();
    }

    protected async Task<List<CartProduct>> SaveCartProducts(int n, Guid? ownerId = null)
    {
        ICartProductRepository db = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        await db.ClearCartProducts(ownerId.GetValueOrDefault(TestAuthHandler.UserId));
        List<CartProduct> result = new(n);
        for (int i = 0; i < n; i++)
        {
            CartProduct product = GetRandomCartProduct(i * 1000);
            result.Add(product);
            await db.AddCartProduct(product, ownerId.GetValueOrDefault(TestAuthHandler.UserId));
        }
        return result;

        static CartProduct GetRandomCartProduct(int order = 0)
        {
            return new CartProduct()
            {
                Amount = Random.Shared.NextDouble() * 10,
                Name = "Product" + Random.Shared.Next(0, 1000000),
                Order = order,
                UnitPrice = Random.Shared.NextDouble() * 10,
            };
        }
    }

    protected async Task<List<StoreProduct>> SaveStoreProducts(int n)
    {
        IStoreProductRepository db = _scope.ServiceProvider.GetRequiredService<IStoreProductRepository>();
        List<StoreProduct> result = new(n);
        for (int i = 0; i < n; i++)
        {
            StoreProduct product = GetRandomStoreProduct();
            result.Add(product);
            await db.AddProduct(product, TestAuthHandler.UserId);
        }
        return result;

        static StoreProduct GetRandomStoreProduct()
        {
            return new StoreProduct()
            {
                Name = "Product" + Random.Shared.Next(0, 1000000),
                UnitPrice = Math.Round(Random.Shared.NextDouble() * 10, 2),
            };
        }
    }

    protected async Task ClearStoreProducts()
    {
        IStoreProductRepository db = _scope.ServiceProvider.GetRequiredService<IStoreProductRepository>();
        await db.DeleteAll(TestAuthHandler.UserId);
    }


    protected async Task<Guid> CreateNewGroup(bool randomUser = false)
    {
        ICartGroupRepository groupRepository = _scope.ServiceProvider.GetRequiredService<ICartGroupRepository>();
        Response<Guid, NotFoundError> response = await groupRepository.CreateGroup(Guid.NewGuid().ToString().Replace("-", ""), new HashSet<string>() { randomUser ? TestAuthHandler.RandomEmail2 : TestAuthHandler.UserEmail, TestAuthHandler.RandomEmail1 });
        return response.Match(x => x, e => throw new Exception());
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
