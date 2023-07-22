using GroceryListHelper.Core.Exceptions;
using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.Core.RepositoryContracts;

public interface IStoreProductRepository
{
    Task<List<StoreProduct>> GetStoreProductsForUser(Guid userId);
    Task<ConflictError?> AddProduct(StoreProduct product, Guid userId);
    Task DeleteAll(Guid userId);
    Task<NotFoundError?> UpdatePrice(string productName, Guid userId, double price);
}
