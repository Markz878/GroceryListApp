using ApiIntegrationTests;
using GroceryListHelper.DataAccess;
using GroceryListHelper.Shared.Models.CartProduct;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ApiIntegrationTests.EndpointTests;

public class GetCartProductsTests : BaseTest
{
    public GetCartProductsTests(WebApplicationFactoryFixture factory, ITestOutputHelper testOutputHelper) : base(factory, testOutputHelper)
    {
    }

    [Fact]
    public async Task GetCartProducts()
    {
        GroceryStoreDbContext db = GetDbContext();
        db.CartProducts.AddRange(GenerateCartProducts(5));
        await db.SaveChangesAsync();

        List<CartProductCollectable>? response = await _client.GetFromJsonAsync<List<CartProductCollectable>>("api/cartproducts");
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Equal(5, response.Count);
    }
}
