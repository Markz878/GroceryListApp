using GroceryListHelper.Core.Domain.CartProducts;
using GroceryListHelper.Core.Domain.HelperModels;
using GroceryListHelper.Core.Domain.StoreProducts;
using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Core.Features.CartProducts;
using GroceryListHelper.Core.Features.StoreProducts;
using System.Text.Json;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

public abstract class BaseTest : IAsyncLifetime
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


    public virtual async Task InitializeAsync()
    {
        HttpResponseMessage csrfTokenResponse = await _client.GetAsync("api/account/token");
        IEnumerable<string> setCookieHeaders = csrfTokenResponse.Headers.GetValues("Set-Cookie");
        string xsrfTokenHeaderValue = setCookieHeaders.First(x => x.StartsWith("XSRF-TOKEN"));
        int i1 = xsrfTokenHeaderValue.IndexOf('=') + 1;
        int i2 = xsrfTokenHeaderValue.IndexOf(';');
        string xsrfToken = xsrfTokenHeaderValue[i1..i2];
        _client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", xsrfToken);
    }

    protected async Task<List<CartProduct>> SaveCartProducts(int n, Guid? ownerId = null)
    {
        await _mediator.Send(new ClearUserCartProductsCommand() { UserId = TestAuthHandler.UserId });
        List<CartProduct> products = new(n);
        for (int i = 0; i < n; i++)
        {
            CartProduct product = GetRandomCartProduct(i * 1000);
            products.Add(product);
        }
        await _mediator.Send(new UpsertCartProductsCommand() { CartProducts = products, UserId = ownerId.GetValueOrDefault(TestAuthHandler.UserId) });
        return products;

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
        List<StoreProduct> storeProducts = new(n);
        for (int i = 0; i < n; i++)
        {
            StoreProduct product = GetRandomStoreProduct();
            storeProducts.Add(product);
        }
        await _mediator.Send(new UpsertStoreProductsCommand() { StoreProducts = storeProducts, UserId = TestAuthHandler.UserId });
        return storeProducts;

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

    public virtual Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }
}
