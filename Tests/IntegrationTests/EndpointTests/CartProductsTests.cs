using GroceryListHelper.Tests.IntegrationTests.Infrastucture;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

public sealed class CartProductsTests : BaseTest
{
    public CartProductsTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
    }

    [Fact]
    public async Task GetCartProducts()
    {
        List<CartProductDbModel> insertedProducts = await SaveCartProducts(1);
        HttpResponseMessage response = await _client.GetAsync("api/cartproducts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<CartProductCollectable>? products = await response.Content.ReadFromJsonAsync<List<CartProductCollectable>>(_jsonOptions);
        Assert.NotNull(products);
        Assert.Equal(insertedProducts[0].Amount, products[0].Amount);
        Assert.Equal(insertedProducts[0].Name, products[0].Name);
        Assert.Equal(insertedProducts[0].Order, products[0].Order);
        Assert.Equal(insertedProducts[0].UnitPrice, products[0].UnitPrice);
    }

    [Fact]
    public async Task AddCartProduct_Success_ReturnsCreated()
    {
        CartProduct cartProduct = new()
        {
            Amount = Random.Shared.NextDouble() * 10,
            Name = "Product" + Random.Shared.Next(0, 10000),
            Order = 1500,
            UnitPrice = Random.Shared.NextDouble() * 10
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/cartproducts", cartProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        string id = await response.Content.ReadAsStringAsync();
        Assert.True(Guid.TryParse(id.Trim('"'), out _));
    }

    [Fact]
    public async Task AddCartProduct_InvalidProduct_ReturnsBadRequest()
    {
        CartProduct cartProduct = new()
        {
            Amount = -Random.Shared.NextDouble() * 10,
            Name = new string('x', 31),
            Order = 1500,
            UnitPrice = -Random.Shared.NextDouble() * 10
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/cartproducts", cartProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string responseString = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(responseString));
    }

    [Fact]
    public async Task DeleteCartProducts_Success_ReturnsNoContent()
    {
        await SaveCartProducts(1);
        HttpResponseMessage response = await _client.DeleteAsync("api/cartproducts");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        List<CartProductDbModel> products = await db.CartProducts.ToListAsync();
        Assert.Empty(products);
    }

    [Fact]
    public async Task DeleteCartProduct_Success_ReturnsNoContent()
    {
        List<CartProductDbModel> savedProducts = await SaveCartProducts(3);
        HttpResponseMessage response = await _client.DeleteAsync($"api/cartproducts/{savedProducts[1].Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        List<CartProductDbModel> products = await db.CartProducts.ToListAsync();
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task DeleteCartProduct_InvalidGuid_ReturnsNotFound()
    {
        await SaveCartProducts(3);
        HttpResponseMessage response = await _client.DeleteAsync($"api/cartproducts/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        List<CartProductDbModel> products = await db.CartProducts.ToListAsync();
        Assert.Equal(3, products.Count);
    }

    [Fact]
    public async Task DeleteCartProduct_NotUsersProduct_ReturnsNotFound()
    {
        List<CartProductDbModel> savedProducts = await SaveCartProducts(3, true);
        HttpResponseMessage response = await _client.DeleteAsync($"api/cartproducts/{savedProducts[1].Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        List<CartProductDbModel> products = await db.CartProducts.ToListAsync();
        Assert.Equal(3, products.Count);
    }

    [Fact]
    public async Task UpdateCartProduct_Success_ReturnsOk()
    {
        List<CartProductDbModel> insertedProducts = await SaveCartProducts(1);
        CartProductCollectable cartProduct = new()
        {
            Id = insertedProducts[0].Id,
            Amount = insertedProducts[0].Amount + 1,
            Name = insertedProducts[0].Name + 'A',
            Order = insertedProducts[0].Order + 1000,
            UnitPrice = insertedProducts[0].UnitPrice + 1,
            IsCollected = !insertedProducts[0].IsCollected
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync("api/cartproducts", cartProduct);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        CartProductDbModel product = await db.CartProducts.FirstAsync(x => x.Id == insertedProducts[0].Id);
        Assert.Equal(cartProduct.Id, product.Id);
        Assert.Equal(cartProduct.Amount, product.Amount);
        Assert.Equal(cartProduct.Name, product.Name);
        Assert.Equal(cartProduct.Order, product.Order);
        Assert.Equal(cartProduct.UnitPrice, product.UnitPrice);
        Assert.Equal(cartProduct.IsCollected, product.IsCollected);
    }

    [Fact]
    public async Task UpdateCartProduct_InvalidProduct_ReturnsBadRequest()
    {
        List<CartProductDbModel> insertedProducts = await SaveCartProducts(1);
        CartProductCollectable cartProduct = new()
        {
            Id = insertedProducts[0].Id,
            Amount = -Random.Shared.NextDouble() * 10,
            Name = new string('x', 31),
            Order = 1500,
            UnitPrice = -Random.Shared.NextDouble() * 10,
            IsCollected = !insertedProducts[0].IsCollected
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync("api/cartproducts", cartProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCartProduct_InvalidProductId_ReturnsNotFound()
    {
        List<CartProductDbModel> insertedProducts = await SaveCartProducts(1);
        CartProductCollectable cartProduct = new()
        {
            Id = Guid.NewGuid(),
            Amount = insertedProducts[0].Amount + 1,
            Name = insertedProducts[0].Name + 'A',
            Order = insertedProducts[0].Order + 1000,
            UnitPrice = insertedProducts[0].UnitPrice + 1,
            IsCollected = !insertedProducts[0].IsCollected
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync("api/cartproducts", cartProduct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        GroceryStoreDbContext db = scope.ServiceProvider.GetRequiredService<GroceryStoreDbContext>();
        CartProductDbModel product = await db.CartProducts.FirstAsync(x => x.Id == insertedProducts[0].Id);
    }
}
