using System.ComponentModel;

namespace GroceryListHelper.Client.Services;

public sealed class CartProductsLocalService : ICartProductsService
{
    private readonly IndexViewModel viewModel;
    private readonly ILocalStorageService localStorage;
    private const string cartProductsKey = "cartProducts";

    public CartProductsLocalService(IndexViewModel viewModel, ILocalStorageService localStorage)
    {
        this.viewModel = viewModel;
        this.localStorage = localStorage;
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
    }

    public async Task<Guid> SaveCartProduct(CartProduct product)
    {
        await localStorage.SetItemAsync(cartProductsKey, viewModel.CartProducts);
        return viewModel.CartProducts.Last().Id;
    }

    public async Task DeleteCartProduct(Guid id)
    {
        await localStorage.SetItemAsync(cartProductsKey, viewModel.CartProducts);
    }

    public async Task DeleteAllCartProducts()
    {
        await localStorage.RemoveItemAsync(cartProductsKey);
    }

    public async Task UpdateCartProduct(CartProductUIModel cartProduct)
    {
        await localStorage.SetItemAsync(cartProductsKey, viewModel.CartProducts);
    }

    public async Task SortCartProducts(ListSortDirection sortDirection)
    {
        await localStorage.SetItemAsync(cartProductsKey, viewModel.CartProducts);
    }
}
