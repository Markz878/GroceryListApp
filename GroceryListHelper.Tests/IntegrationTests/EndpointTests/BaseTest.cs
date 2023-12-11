using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Core.Features.CartProducts;
using GroceryListHelper.Core.Features.StoreProducts;
using GroceryListHelper.Shared.Models.StoreProducts;
using System.Text.Json;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

public abstract class BaseTest : IDisposable
{
    protected readonly WebApplicationFactoryFixture _factory;
    protected readonly HttpClient _client;
    protected static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    protected readonly IServiceScope _scope;
    protected readonly IMediator _mediator;

    protected BaseTest(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        factory.TestOutputHelper = testOutputHelper;
        _client = factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    protected async Task<List<CartProduct>> SaveCartProducts(int n, Guid? ownerId = null)
    {
        await _mediator.Send(new ClearCartProductsCommand() { UserId = TestAuthHandler.UserId });
        List<CartProduct> result = new(n);
        for (int i = 0; i < n; i++)
        {
            CartProduct product = GetRandomCartProduct(i * 1000);
            result.Add(product);
            await _mediator.Send(new AddCartProductCommand() { CartProduct = product, UserId = ownerId.GetValueOrDefault(TestAuthHandler.UserId) });
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
        List<StoreProduct> result = new(n);
        for (int i = 0; i < n; i++)
        {
            StoreProduct product = GetRandomStoreProduct();
            result.Add(product);
            await _mediator.Send(new AddStoreProductCommand() { Product = product, UserId = TestAuthHandler.UserId });
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
        await _mediator.Send(new DeleteAllUserStoreProductsCommand() { UserId = TestAuthHandler.UserId });
    }


    protected async Task<Guid> CreateNewGroup(bool randomUser = false)
    {
        Result<Guid, NotFoundError> response = await _mediator.Send(new CreateGroupCommand() { GroupName = Guid.NewGuid().ToString().Replace("-", ""), UserEmails = [randomUser ? TestAuthHandler.RandomEmail2 : TestAuthHandler.UserEmail, TestAuthHandler.RandomEmail1] });
        return response.Map(x => x, e => throw new Exception());
    }

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);
    }
}
