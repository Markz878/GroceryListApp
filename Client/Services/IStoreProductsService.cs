using GroceryListHelper.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services;

public interface IStoreProductsService
{
    Task<bool> ClearStoreProducts();
    Task<List<StoreProductUIModel>> GetStoreProducts();
    Task<bool> SaveStoreProduct(StoreProductUIModel product);
    Task<bool> UpdateStoreProductPrice(int id, double price);
}