using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared.Models.StoreProduct;
using Microsoft.EntityFrameworkCore;

namespace GroceryListHelper.DataAccess.Repositories;

public class StoreProductRepository : IStoreProductRepository
{
    private readonly GroceryStoreDbContext db;

    public StoreProductRepository(GroceryStoreDbContext db)
    {
        this.db = db;
    }

    public async Task<StoreProductServerModel> GetStoreProductForUser(string productId, string userId)
    {
        StoreProductDbModel dbProduct = await db.StoreProducts.SingleOrDefaultAsync(x => x.Id == productId && x.UserId == userId);
        return new StoreProductServerModel()
        {
            Id = dbProduct.Id,
            Name = dbProduct.Name,
            UnitPrice = dbProduct.UnitPrice,
        };
    }

    public async Task<List<StoreProductServerModel>> GetStoreProductsForUser(string userId)
    {
        return await db.StoreProducts.Where(x => x.UserId == userId).Select(x => new StoreProductServerModel()
        {
            Id = x.Id,
            Name = x.Name,
            UnitPrice = x.UnitPrice,
        }).ToListAsync();
    }

    public async Task<string> AddProduct(StoreProductModel product, string userId)
    {
        StoreProductDbModel storeProduct = new() { Name = product.Name, UnitPrice = product.UnitPrice, UserId = userId };
        db.StoreProducts.Add(storeProduct);
        await db.SaveChangesAsync();
        return storeProduct.Id;
    }

    public Task DeleteAll(string userId)
    {
        db.StoreProducts.RemoveRange(db.StoreProducts.Where(x => x.UserId == userId));
        return db.SaveChangesAsync();
    }

    public async Task<bool> DeleteItem(string productId, string userId)
    {
        StoreProductDbModel product = db.StoreProducts.Find(productId, userId);
        if (product == null || product.UserId != userId)
        {
            return false;
        }
        db.StoreProducts.Remove(product);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdatePrice(string productId, string userId, double price)
    {
        StoreProductDbModel product = await db.StoreProducts.FindAsync(productId, userId);
        if (product == null || product.UserId != userId)
        {
            return false;
        }
        product.UnitPrice = price;
        return await db.SaveChangesAsync() > 0;
    }
}
