using Azure;
using Azure.Data.Tables;
using GroceryListHelper.DataAccess.Exceptions;
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
        AsyncPageable<StoreProductDbModel> cartProductPages = db.QueryAsync<StoreProductDbModel>(x => x.PartitionKey == userId.ToString());
        List<StoreProduct> result = new();
        string? token = null;
        await foreach (Page<StoreProductDbModel> cartProductPage in cartProductPages.AsPages())
        {
            token = cartProductPage.ContinuationToken;
            result.AddRange(cartProductPage.Values.Select(x => new StoreProduct()
            {
                Name = x.Name,
                UnitPrice = x.UnitPrice
            }));
        }
        return result;
    }

    public async Task AddProduct(StoreProduct product, Guid userId)
    {
        StoreProductDbModel storeProduct = new() { Name = product.Name, UnitPrice = product.UnitPrice, OwnerId = userId };
        await db.AddEntityAsync(storeProduct);
    }

    public async Task DeleteAll(Guid userId)
    {
        AsyncPageable<StoreProductDbModel> pages = db.QueryAsync<StoreProductDbModel>(x => x.PartitionKey == userId.ToString());
        string? token = null;
        List<StoreProductDbModel> products = new();
        await foreach (Page<StoreProductDbModel> page in pages.AsPages(token))
        {
            token = page.ContinuationToken;
            products.AddRange(page.Values);
        }
        Response<IReadOnlyList<Response>> response = await db.SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
    }

    public async Task<Exception?> UpdatePrice(string productName, Guid userId, double price)
    {
        try
        {
            Response response = await db.UpdateEntityAsync(new StoreProductDbModel() { Name = productName, OwnerId = userId, UnitPrice = price }, ETag.All);
            return null;
        }
        catch (RequestFailedException ex) when (ex.Status is 404)
        {
            return NotFoundException.ForType<StoreProductDbModel>();
        }
    }
}
