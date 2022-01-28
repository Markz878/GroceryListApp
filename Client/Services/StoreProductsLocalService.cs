using Blazored.LocalStorage;
using GroceryListHelper.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services;

public class StoreProductsLocalService : IStoreProductsService
{
    private readonly ILocalStorageService localStorage;
    private const string storeProductsKey = "storeProducts";

    public StoreProductsLocalService(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task<List<StoreProductUIModel>> GetStoreProducts()
    {
        return await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey) ?? new List<StoreProductUIModel>();
    }

    public async Task SaveStoreProduct(StoreProductUIModel product)
    {
        List<StoreProductUIModel> products = await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey) ?? new List<StoreProductUIModel>();
        products.Add(product);
        await localStorage.SetItemAsync(storeProductsKey, products);
    }

    public async Task ClearStoreProducts()
    {
        await localStorage.RemoveItemAsync(storeProductsKey);
    }

    public async Task UpdateStoreProductPrice(int id, double price)
    {
        List<StoreProductUIModel> products = await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey);
        StoreProductUIModel product = products.Find(x => x.Id == id);
        product.UnitPrice = price;
        await localStorage.SetItemAsync(storeProductsKey, products);
    }
}
