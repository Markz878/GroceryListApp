namespace GroceryListHelper.Client.Services;

public sealed class StoreProductsLocalService : IStoreProductsService
{
    private readonly ILocalStorageService localStorage;
    private const string storeProductsKey = "storeProducts";

    public StoreProductsLocalService(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task<List<StoreProduct>> GetStoreProducts()
    {
        return await localStorage.GetItemAsync<List<StoreProduct>>(storeProductsKey) ?? new List<StoreProduct>();
    }

    public async Task SaveStoreProduct(StoreProduct product)
    {
        List<StoreProduct> products = await localStorage.GetItemAsync<List<StoreProduct>>(storeProductsKey) ?? new List<StoreProduct>();
        StoreProduct newProduct = new()
        {
            Name = product.Name,
            UnitPrice = product.UnitPrice
        };
        products.Add(newProduct);
        await localStorage.SetItemAsync(storeProductsKey, products);
    }

    public async Task ClearStoreProducts()
    {
        await localStorage.RemoveItemAsync(storeProductsKey);
    }

    public async Task UpdateStoreProduct(StoreProduct storeProduct)
    {
        List<StoreProduct> products = await localStorage.GetItemAsync<List<StoreProduct>>(storeProductsKey);
        StoreProduct? product = products.Find(x => x.Name == storeProduct.Name);
        if (product != null)
        {
            product.UnitPrice = storeProduct.UnitPrice;
            product.Name = storeProduct.Name;
            await localStorage.SetItemAsync(storeProductsKey, products);
        }
    }
}
