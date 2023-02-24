using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.DataAccess.Repositories;

public interface IStoreProductRepository
{
    Task<List<StoreProduct>> GetStoreProductsForUser(Guid userId);
    Task AddProduct(StoreProduct product, Guid userId);
    Task DeleteAll(Guid userId);
    Task<NotFound?> UpdatePrice(string productName, Guid userId, double price);
}
