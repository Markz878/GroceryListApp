using Azure;
using Azure.Data.Tables;
using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared.Models.StoreProducts;

namespace GroceryListHelper.DataAccess.Repositories;

public sealed class StoreProductRepository : IStoreProductRepository
{
    private readonly TableClient db;

    public StoreProductRepository(TableServiceClient db)
    {
        this.db = db.GetTableClient(StoreProductDbModel.GetTableName());
    }

    public async Task<List<StoreProduct>> GetStoreProductsForUser(Guid userId)
    {
        List<StoreProductDbModel> result = await db.GetTableEntitiesByPrimaryKey<StoreProductDbModel>(userId.ToString());
        return result.Select(x => new StoreProduct() { Name = x.Name, UnitPrice = x.UnitPrice }).ToList();
    }

    public async Task AddProduct(StoreProduct product, Guid userId)
    {
        StoreProductDbModel storeProduct = new() { Name = product.Name, UnitPrice = product.UnitPrice, OwnerId = userId };
        await db.AddEntityAsync(storeProduct);
    }

    public async Task DeleteAll(Guid userId)
    {
        List<StoreProductDbModel> products = await db.GetTableEntitiesByPrimaryKey<StoreProductDbModel>(userId.ToString());
        if (products.Count > 0)
        {
            await db.SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
        }
    }

    public async Task<Exception?> UpdatePrice(string productName, Guid userId, double price)
    {
        try
        {
            await db.UpdateEntityAsync(new StoreProductDbModel() { Name = productName, OwnerId = userId, UnitPrice = price }, ETag.All);
            return null;
        }
        catch (RequestFailedException ex) when (ex.Status is 404)
        {
            return NotFoundException.ForType<StoreProductDbModel>();
        }
    }
}
