using GroceryListHelper.Core.Domain.StoreProducts;
using GroceryListHelper.Core.Features.StoreProducts;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class StoreProductsTests : BaseTest
{
    private const string _uri = "api/storeproducts";
    public StoreProductsTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
    }

    [Fact]
    public async Task GetStoreProducts()
    {
        await ClearStoreProducts();
        List<StoreProduct> insertedProducts = await SaveStoreProducts(1);
        HttpResponseMessage response = await _client.GetAsync(_uri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<StoreProduct>? products = await response.Content.ReadFromJsonAsync<List<StoreProduct>>(_jsonOptions);
        Assert.NotNull(products);
        Assert.Equal(insertedProducts[0].Name, products[0].Name);
        Assert.Equal(insertedProducts[0].UnitPrice, products[0].UnitPrice);
    }

    [Fact]
    public async Task AddStoreProduct_Success_ReturnsCreated()
    {
        StoreProduct storeProduct = new()
        {
            Name = "Product" + Random.Shared.Next(0, 10000),
            UnitPrice = Random.Shared.NextDouble() * 10
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, storeProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AddStoreProduct_InvalidProduct_ReturnsBadRequest()
    {
        StoreProduct storeProduct = new()
        {
            Name = new string('x', 31),
            UnitPrice = -Random.Shared.NextDouble() * 10
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, storeProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string responseString = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(responseString));
    }

    [Fact]
    public async Task DeleteStoreProduct_Success_ReturnsNoContent()
    {
        await SaveStoreProducts(1);
        HttpResponseMessage response = await _client.DeleteAsync(_uri);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        List<StoreProduct> products = await _mediator.Send(new GetUserStoreProductsQuery() { UserId = TestAuthHandler.UserId });
        Assert.Empty(products);
    }

    [Fact]
    public async Task UpdateStoreProduct_Success_ReturnsOk()
    {
        List<StoreProduct> insertedProducts = await SaveStoreProducts(1);
        StoreProduct storeProduct = new()
        {
            Name = insertedProducts[0].Name,
            UnitPrice = insertedProducts[0].UnitPrice + 1,
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, storeProduct);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        List<StoreProduct> storeProducts = await _mediator.Send(new GetUserStoreProductsQuery() { UserId = TestAuthHandler.UserId });
        StoreProduct product = storeProducts.First(x => x.Name == storeProduct.Name);
        Assert.Equal(storeProduct.UnitPrice, product.UnitPrice);
    }

    [Fact]
    public async Task UpdateStoreProduct_InvalidProduct_ReturnsBadRequest()
    {
        StoreProduct storeProduct = new()
        {
            Name = new string('x', 31),
            UnitPrice = -Random.Shared.NextDouble() * 10,
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, storeProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStoreProduct_InvalidProductName_ReturnsNotFound()
    {
        List<StoreProduct> insertedProducts = await SaveStoreProducts(1);
        StoreProduct storeProduct = new()
        {
            Name = "Test",
            UnitPrice = insertedProducts[0].UnitPrice + 1,
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, storeProduct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
