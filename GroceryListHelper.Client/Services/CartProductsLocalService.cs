namespace GroceryListHelper.Client.Services;

public sealed class CartProductsLocalService(ILocalStorageService localStorage, AppState appState) : ICartProductsService
{
    private const string cartProductsKey = "cartProducts";

    public async Task<List<CartProductCollectable>> GetCartProducts()
    {
        List<CartProductCollectable> products = await localStorage.GetItemAsync<List<CartProductCollectable>>(cartProductsKey) ?? [];
        return products;
    }

    public async Task CreateCartProduct(CartProduct product)
    {
        await localStorage.SetItemAsync(cartProductsKey, appState.CartProducts);
    }

    public async Task DeleteCartProduct(string name)
    {
        await localStorage.SetItemAsync(cartProductsKey, appState.CartProducts);
    }

    public async Task DeleteAllCartProducts()
    {
        await localStorage.RemoveItemAsync(cartProductsKey);
    }

    public async Task UpdateCartProduct(CartProductCollectable cartProduct)
    {
        await localStorage.SetItemAsync(cartProductsKey, appState.CartProducts);
    }

    public async Task SortCartProducts(ListSortDirection sortDirection)
    {
        await localStorage.SetItemAsync(cartProductsKey, appState.CartProducts);
    }
}
