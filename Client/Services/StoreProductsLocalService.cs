using Blazored.LocalStorage;
using GroceryListHelper.Client.Models;

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

    public async Task<bool> SaveStoreProduct(StoreProductUIModel product)
    {
        try
        {
            List<StoreProductUIModel> products = await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey) ?? new List<StoreProductUIModel>();
            products.Add(product);
            await localStorage.SetItemAsync(storeProductsKey, products);
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public async Task<bool> ClearStoreProducts()
    {
        try
        {
            await localStorage.RemoveItemAsync(storeProductsKey);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateStoreProductPrice(string id, double price)
    {
        try
        {
            List<StoreProductUIModel> products = await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey);
            StoreProductUIModel product = products.Find(x => x.Id == id);
            product.UnitPrice = price;
            await localStorage.SetItemAsync(storeProductsKey, products);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
