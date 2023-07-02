namespace GroceryListHelper.Client.Services;

public sealed class CartProductsGroupService : ICartProductsService
{
    private readonly HttpClient client;
    private readonly string uri;

    public CartProductsGroupService(IHttpClientFactory clientFactory, NavigationManager navigation)
    {
        client = clientFactory.CreateClient("ProtectedClient");
        string groupId = navigation.Uri[navigation.Uri.LastIndexOf('/')..];
        uri = $"api/cartgroupproducts{groupId}";
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await client.GetFromJsonAsync<List<CartProductUIModel>>(uri) ?? new List<CartProductUIModel>();
    }

    public async Task SaveCartProduct(CartProduct product)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCartProduct(string productName)
    {
        HttpResponseMessage response = await client.DeleteAsync(uri + $"/{productName}");
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAllCartProducts()
    {
        await client.DeleteAsync(uri);
    }

    public async Task UpdateCartProduct(CartProductCollectable cartProduct)
    {
        await client.PutAsJsonAsync(uri, cartProduct);
    }

    public async Task SortCartProducts(ListSortDirection sortDirection)
    {
        await client.PatchAsync(uri + "/sort/" + (int)sortDirection, null);
    }
}
