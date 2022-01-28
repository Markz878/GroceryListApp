using GroceryListHelper.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services;

public interface IStoreProductsService
{
    Task ClearStoreProducts();
    Task<List<StoreProductUIModel>> GetStoreProducts();
    Task SaveStoreProduct(StoreProductUIModel product);
    Task UpdateStoreProductPrice(int id, double price);
}