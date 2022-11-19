using GroceryListHelper.Shared.Models.StoreProduct;

namespace ApiIntegrationTests.EndpointTests;

public class StoreProductsTests : BaseTest
{
    public StoreProductsTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
    }

    [Fact]
    public async Task GetStoreProducts()
    {
        List<StoreProductDbModel> insertedProducts = await SaveStoreProducts(1);
        HttpResponseMessage response = await _client.GetAsync("api/storeproducts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<StoreProductServerModel>? products = await response.Content.ReadFromJsonAsync<List<StoreProductServerModel>>(_jsonOptions);
        Assert.NotNull(products);
        Assert.Equal(insertedProducts[0].Name, products[0].Name);
        Assert.Equal(insertedProducts[0].UnitPrice, products[0].UnitPrice);
    }

    [Fact]
    public async Task AddStoreProduct_Success_ReturnsCreated()
    {
        StoreProductModel storeProduct = new()
        {
            Name = "Product" + Random.Shared.Next(0, 10000),
            UnitPrice = Random.Shared.NextDouble() * 10
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/storeproducts", storeProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        string id = await response.Content.ReadAsStringAsync();
        Assert.True(Guid.TryParse(id.Trim('"'), out _));
    }

    [Fact]
    public async Task AddStoreProduct_InvalidProduct_ReturnsBadRequest()
    {
        StoreProductModel storeProduct = new()
        {
            Name = new string('x', 31),
            UnitPrice = -Random.Shared.NextDouble() * 10
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/storeproducts", storeProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string responseString = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(responseString));
    }

    [Fact]
    public async Task DeleteCartProduct_Success_ReturnsNoContent()
    {
        await SaveStoreProducts(1);
        HttpResponseMessage response = await _client.DeleteAsync("api/storeProducts");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        List<StoreProductDbModel> products = await db.StoreProducts.ToListAsync();
        Assert.Empty(products);
    }

    [Fact]
    public async Task UpdateStoreProduct_Success_ReturnsOk()
    {
        List<StoreProductDbModel> insertedProducts = await SaveStoreProducts(1);
        StoreProductServerModel storeProduct = new()
        {
            Id = insertedProducts[0].Id,
            Name = insertedProducts[0].Name,
            UnitPrice = insertedProducts[0].UnitPrice + 1,
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync("api/storeproducts", storeProduct);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        StoreProductDbModel product = await db.StoreProducts.FirstAsync(x=>x.Id == insertedProducts[0].Id);
        Assert.Equal(storeProduct.Id, product.Id);
        Assert.Equal(storeProduct.Name, product.Name);
        Assert.Equal(storeProduct.UnitPrice, product.UnitPrice);
    }

    [Fact]
    public async Task UpdateStoreProduct_InvalidProduct_ReturnsBadRequest()
    {
        List<StoreProductDbModel> insertedProducts = await SaveStoreProducts(1);
        StoreProductServerModel storeProduct = new()
        {
            Id = insertedProducts[0].Id,
            Name = new string('x', 31),
            UnitPrice = -Random.Shared.NextDouble() * 10,
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync("api/storeproducts", storeProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStoreProduct_InvalidProductId_ReturnsNotFound()
    {
        List<StoreProductDbModel> insertedProducts = await SaveStoreProducts(1);
        StoreProductServerModel storeProduct = new()
        {
            Id = Guid.NewGuid(),
            Name = insertedProducts[0].Name + 'A',
            UnitPrice = insertedProducts[0].UnitPrice + 1,
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync("api/storeproducts", storeProduct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        StoreProductDbModel product = await db.StoreProducts.FirstAsync(x => x.Id == insertedProducts[0].Id);
    }
}
