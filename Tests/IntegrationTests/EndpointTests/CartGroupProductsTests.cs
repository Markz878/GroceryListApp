using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared.Models.HelperModels;
using GroceryListHelper.Tests.IntegrationTests.Infrastucture;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;
public class CartGroupProductsTests : BaseTest, IAsyncLifetime
{
    private string _uri = "api/cartgroupproducts/";
    private Guid groupId;

    public CartGroupProductsTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
    }


    [Fact]
    public async Task GetCartProducts()
    {
        List<CartProduct> insertedProducts = await SaveCartProducts(1, groupId);
        HttpResponseMessage response = await _client.GetAsync(_uri);
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
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, cartProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        ICartProductRepository repository = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        List<CartProductCollectable> products = await repository.GetCartProducts(groupId);
        Assert.Contains(products, x =>
            x.Name == cartProduct.Name &&
            x.Amount == cartProduct.Amount &&
            x.Order == cartProduct.Order &&
            x.UnitPrice == cartProduct.UnitPrice);
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
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, cartProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string responseString = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(responseString));
        Assert.Contains("The length of 'Name' must be 30 characters or fewer. You entered 31 characters.", responseString);
        Assert.Contains("'Amount' must be between 0", responseString);
        Assert.Contains("'Unit Price' must be between 0 and", responseString);
    }

    [Fact]
    public async Task DeleteCartProducts_Success_ReturnsNoContent()
    {
        await SaveCartProducts(Random.Shared.Next(1, 5), groupId);
        HttpResponseMessage response = await _client.DeleteAsync(_uri);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        ICartProductRepository repository = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        List<CartProductCollectable> products = await repository.GetCartProducts(groupId);
        Assert.Empty(products);
    }

    [Fact]
    public async Task DeleteCartProduct_Success_ReturnsNoContent()
    {
        List<CartProduct> savedProducts = await SaveCartProducts(3, groupId);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{savedProducts[1].Name}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        ICartProductRepository repository = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        List<CartProductCollectable> products = await repository.GetCartProducts(groupId);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task DeleteCartProduct_InvalidGuid_ReturnsNotFound()
    {
        await SaveCartProducts(3, groupId);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/XXX");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        ICartProductRepository repository = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        List<CartProductCollectable> products = await repository.GetCartProducts(groupId);
        Assert.Equal(3, products.Count);
    }

    [Fact]
    public async Task DeleteCartProduct_NotUsersProduct_ReturnsNotFound()
    {
        Guid invalidGroupId = Guid.NewGuid();
        List<CartProduct> savedProducts = await SaveCartProducts(3, invalidGroupId);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{savedProducts[1].Name}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        ICartProductRepository repository = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        List<CartProductCollectable> products = await repository.GetCartProducts(invalidGroupId);
        Assert.Equal(3, products.Count);
    }

    [Fact]
    public async Task UpdateCartProduct_Success_ReturnsOk()
    {
        List<CartProduct> insertedProducts = await SaveCartProducts(1, groupId);
        CartProductCollectable cartProduct = new()
        {
            Amount = insertedProducts[0].Amount + 1,
            Name = insertedProducts[0].Name,
            Order = insertedProducts[0].Order + 1000,
            UnitPrice = insertedProducts[0].UnitPrice + 1,
            IsCollected = true
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, cartProduct);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        ICartProductRepository repository = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        List<CartProductCollectable> products = await repository.GetCartProducts(groupId);
        CartProductCollectable product = products.First(x => x.Name == cartProduct.Name);
        Assert.Equal(cartProduct.Amount, product.Amount);
        Assert.Equal(cartProduct.Name, product.Name);
        Assert.Equal(cartProduct.Order, product.Order);
        Assert.Equal(cartProduct.UnitPrice, product.UnitPrice);
        Assert.Equal(cartProduct.IsCollected, product.IsCollected);
    }

    [Fact]
    public async Task UpdateCartProduct_InvalidProduct_ReturnsBadRequest()
    {
        await SaveCartProducts(1, groupId);
        CartProductCollectable cartProduct = new()
        {
            Amount = -Random.Shared.NextDouble() * 10,
            Name = new string('x', 31),
            Order = 1500,
            UnitPrice = -Random.Shared.NextDouble() * 10,
            IsCollected = true
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, cartProduct);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCartProduct_InvalidProductName_ReturnsNotFound()
    {
        List<CartProduct> insertedProducts = await SaveCartProducts(1, groupId);
        CartProductCollectable cartProduct = new()
        {
            Amount = insertedProducts[0].Amount + 1,
            Name = insertedProducts[0].Name + 'A',
            Order = insertedProducts[0].Order + 1000,
            UnitPrice = insertedProducts[0].UnitPrice + 1,
            IsCollected = true
        };
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, cartProduct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SortCartProducts_OrderAscending()
    {
        List<CartProduct> insertedProducts = await SaveCartProducts(5, groupId);
        HttpResponseMessage response = await _client.PatchAsync($"{_uri}/sort/0", null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        ICartProductRepository repository = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        List<CartProductCollectable> sortedProducts = await repository.GetCartProducts(groupId);
        double order = double.MinValue;
        foreach (CartProductCollectable product in sortedProducts.OrderBy(x => x.Name))
        {
            Assert.True(product.Order > order);
            order = product.Order;
        }
    }

    [Fact]
    public async Task SortCartProducts_OrderDescending()
    {
        List<CartProduct> insertedProducts = await SaveCartProducts(5, groupId);
        HttpResponseMessage response = await _client.PatchAsync($"{_uri}/sort/1", null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        ICartProductRepository repository = _scope.ServiceProvider.GetRequiredService<ICartProductRepository>();
        List<CartProductCollectable> sortedProducts = await repository.GetCartProducts(TestAuthHandler.UserId);
        double order = double.MinValue;
        foreach (CartProductCollectable product in sortedProducts.OrderByDescending(x => x.Name))
        {
            Assert.True(product.Order > order);
            order = product.Order;
        }
    }

    public async Task InitializeAsync()
    {
        ICartGroupRepository groupRepository = _scope.ServiceProvider.GetRequiredService<ICartGroupRepository>();
        Response<Guid, NotFoundException> response = await groupRepository.CreateGroup(Guid.NewGuid().ToString().Replace("-", ""), new HashSet<string>() { TestAuthHandler.UserEmail, TestAuthHandler.RandomEmail1 });
        groupId = response.Match(x => x, e => throw new InvalidOperationException("No user in database"));
        _uri += groupId;
    }

    public async Task DisposeAsync()
    {
        ICartGroupRepository groupRepository = _scope.ServiceProvider.GetRequiredService<ICartGroupRepository>();
        await groupRepository.DeleteCartGroup(groupId, TestAuthHandler.UserEmail);
    }
}
