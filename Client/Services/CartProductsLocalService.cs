using Blazored.LocalStorage;
using GroceryListHelper.Client.Models;

namespace GroceryListHelper.Client.Services;

public class CartProductsLocalService : ICartProductsService
{
    private readonly ILocalStorageService localStorage;
    private const string cartProductsKey = "cartProducts";

    public CartProductsLocalService(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
    }

    public async Task<bool> SaveCartProduct(CartProductUIModel product)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
        products.Add(product);
        await localStorage.SetItemAsync(cartProductsKey, products);
        return true;
    }

    public async Task<bool> DeleteCartProduct(string id)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
        products.Remove(products.Find(x => x.Id == id));
        await localStorage.SetItemAsync(cartProductsKey, products);
        return true;
    }

    public async Task<bool> ClearCartProducts()
    {
        await localStorage.RemoveItemAsync(cartProductsKey);
        return true;
    }

    public async Task<bool> MarkCartProductCollected(string id)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
        CartProductUIModel product = products.Find(x => x.Id == id);
        product.IsCollected = !product.IsCollected;
        await localStorage.SetItemAsync(cartProductsKey, products);
        return true;
    }

    public async Task<bool> UpdateCartProduct(CartProductUIModel cartProduct)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
        CartProductUIModel product = products.Find(x => x.Id == cartProduct.Id);
        product.Name = cartProduct.Name;
        product.Amount = cartProduct.Amount;
        product.UnitPrice = cartProduct.UnitPrice;
        await localStorage.SetItemAsync(cartProductsKey, products);
        return true;
    }
}
