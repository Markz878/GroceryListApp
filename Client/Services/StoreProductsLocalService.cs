using Blazored.LocalStorage;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared.Models.StoreProduct;

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

    public async Task<string> SaveStoreProduct(StoreProductModel product)
    {
        List<StoreProductUIModel> products = await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey) ?? new List<StoreProductUIModel>();
        StoreProductUIModel newProduct = new()
        {
            Name = product.Name,
            UnitPrice = product.UnitPrice
        };
        products.Add(newProduct);
        await localStorage.SetItemAsync(storeProductsKey, products);
        return newProduct.Id;
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

    public async Task<bool> UpdateStoreProduct(StoreProductUIModel storeProduct)
    {
        try
        {
            List<StoreProductUIModel> products = await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey);
            StoreProductUIModel product = products.Find(x => x.Id == storeProduct.Id);
            product.UnitPrice = storeProduct.UnitPrice;
            product.Name = storeProduct.Name;
            await localStorage.SetItemAsync(storeProductsKey, products);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
