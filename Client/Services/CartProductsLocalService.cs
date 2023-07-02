namespace GroceryListHelper.Client.Services;

public sealed class CartProductsLocalService : ICartProductsService
{
    private readonly MainViewModel viewModel;
    private readonly ILocalStorageService localStorage;
    private const string cartProductsKey = "cartProducts";

    public CartProductsLocalService(MainViewModel viewModel, ILocalStorageService localStorage)
    {
        this.viewModel = viewModel;
        this.localStorage = localStorage;
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
    }

    public async Task SaveCartProduct(CartProduct product)
    {
        await localStorage.SetItemAsync(cartProductsKey, viewModel.CartProducts);
    }

    public async Task DeleteCartProduct(string name)
    {
        await localStorage.SetItemAsync(cartProductsKey, viewModel.CartProducts);
    }

    public async Task DeleteAllCartProducts()
    {
        await localStorage.RemoveItemAsync(cartProductsKey);
    }

    public async Task UpdateCartProduct(CartProductCollectable cartProduct)
    {
        await localStorage.SetItemAsync(cartProductsKey, viewModel.CartProducts);
    }

    public async Task SortCartProducts(ListSortDirection sortDirection)
    {
        await localStorage.SetItemAsync(cartProductsKey, viewModel.CartProducts);
    }
}
