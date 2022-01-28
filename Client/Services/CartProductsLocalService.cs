using Blazored.LocalStorage;
using GroceryListHelper.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        product.Id = GetNextId(products);
        products.Add(product);
        await localStorage.SetItemAsync(cartProductsKey, products);
        return true;
    }

    private static int GetNextId(IEnumerable<CartProductUIModel> products)
    {
        int id = 0;
        while (true)
        {
            if (products.Any(x => x.Id == id))
            {
                id++;
            }
            else
            {
                return id;
            }
        }
    }

    public async Task<bool> DeleteCartProduct(int id)
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

    public async Task<bool> MarkCartProductCollected(int id)
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
