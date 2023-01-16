namespace GroceryListHelper.Client.Services;

public sealed class CartProductsApiService : ICartProductsService
{
    private readonly HttpClient client;
    private const string uri = "api/cartproducts";

    public CartProductsApiService(IHttpClientFactory clientFactory)
    {
        client = clientFactory.CreateClient("ProtectedClient");
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await client.GetFromJsonAsync<List<CartProductUIModel>>(uri) ?? new List<CartProductUIModel>();
    }

    public async Task<Guid> SaveCartProduct(CartProduct product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        if (response.IsSuccessStatusCode)
        {
            string textId = await response.Content.ReadAsStringAsync();
            Guid id = Guid.Parse(textId.Trim('"'));
            return id;
        }
        throw new Exception("Could not insert product.");
    }

    public async Task DeleteCartProduct(Guid id)
    {
        HttpResponseMessage response = await client.DeleteAsync(uri + $"/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAllCartProducts()
    {
        await client.DeleteAsync(uri);
    }

    public async Task UpdateCartProduct(CartProductUIModel cartProduct)
    {
        await client.PutAsJsonAsync(uri, cartProduct);
    }
}
