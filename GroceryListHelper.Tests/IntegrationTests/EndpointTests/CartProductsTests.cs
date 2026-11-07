using GroceryListHelper.Core.Domain.CartProducts;
using GroceryListHelper.Core.Features.CartProducts;
using GroceryListHelper.Tests.IntegrationTests.Infrastucture;

namespace GroceryListHelper.Tests.IntegrationTests.EndpointTests;

[Collection(nameof(WebApplicationFactoryCollection))]
public sealed class CartProductsTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : BaseTest(factory, testOutputHelper)
{
    private const string _uri = "api/cartproducts";

    [Fact]
    public async Task GetCartProducts()
    {
        List<CartProduct> insertedProducts = await SaveCartProducts(1);
        HttpResponseMessage response = await _client.GetAsync(_uri, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<CartProduct>? products = await response.Content.ReadFromJsonAsync<List<CartProduct>>(_jsonOptions, cancellationToken: TestContext.Current.CancellationToken);
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
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, cartProduct, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        List<CartProduct> products = await _mediator.Send(new GetUserCartProductsQuery() { UserId = TestAuthHandler.UserId }, TestContext.Current.CancellationToken);
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
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, cartProduct, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        string responseString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrWhiteSpace(responseString));
        Assert.Contains("The length of 'Product Name' must be 30 characters or fewer. You entered 31 characters.", responseString);
        Assert.Contains("'Amount' must be between 0", responseString);
        Assert.Contains("'Unit Price' must be between 0 and", responseString);
    }

    [Fact]
    public async Task DeleteCartProducts_Success_ReturnsNoContent()
    {
        await SaveCartProducts(Random.Shared.Next(1, 5));
        HttpResponseMessage response = await _client.DeleteAsync(_uri, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        List<CartProduct> products = await _mediator.Send(new GetUserCartProductsQuery() { UserId = TestAuthHandler.UserId }, TestContext.Current.CancellationToken);
        Assert.Empty(products);
    }

    [Fact]
    public async Task DeleteCartProduct_Success_ReturnsNoContent()
    {
        List<CartProduct> savedProducts = await SaveCartProducts(3);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{savedProducts[1].Name}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        List<CartProduct> products = await _mediator.Send(new GetUserCartProductsQuery() { UserId = TestAuthHandler.UserId }, TestContext.Current.CancellationToken);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task DeleteCartProduct_InvalidGuid_ReturnsNotFound()
    {
        await SaveCartProducts(3);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/XXX", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        List<CartProduct> products = await _mediator.Send(new GetUserCartProductsQuery() { UserId = TestAuthHandler.UserId }, TestContext.Current.CancellationToken);
        Assert.Equal(3, products.Count);
    }

    [Fact]
    public async Task DeleteCartProduct_NotUsersProduct_ReturnsNotFound()
    {
        Guid fakeUserId = Guid.NewGuid();
        List<CartProduct> savedProducts = await SaveCartProducts(3, fakeUserId);
        HttpResponseMessage response = await _client.DeleteAsync($"{_uri}/{savedProducts[1].Name}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        List<CartProduct> products = await _mediator.Send(new GetUserCartProductsQuery() { UserId = fakeUserId }, TestContext.Current.CancellationToken);
        Assert.Equal(3, products.Count);
    }

    [Fact]
    public async Task SortCartProducts_OrderAscending()
    {
        List<CartProduct> insertedProducts = await SaveCartProducts(5);
        HttpResponseMessage response = await _client.PatchAsync($"{_uri}/sort/0", null, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        List<CartProduct> products = await _mediator.Send(new GetUserCartProductsQuery() { UserId = TestAuthHandler.UserId }, TestContext.Current.CancellationToken);
        double order = double.MinValue;
        foreach (CartProduct product in products.OrderBy(x => x.Name))
        {
            Assert.True(product.Order > order);
            order = product.Order;
        }
    }

    [Fact]
    public async Task SortCartProducts_OrderDescending()
    {
        List<CartProduct> insertedProducts = await SaveCartProducts(5);
        HttpResponseMessage response = await _client.PatchAsync($"{_uri}/sort/1", null, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        List<CartProduct> products = await _mediator.Send(new GetUserCartProductsQuery() { UserId = TestAuthHandler.UserId }, TestContext.Current.CancellationToken);
        double order = double.MinValue;
        foreach (CartProduct product in products.OrderByDescending(x => x.Name))
        {
            Assert.True(product.Order > order);
            order = product.Order;
        }
    }
}
