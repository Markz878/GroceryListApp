using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared.Models.StoreProduct;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GroceryListHelper.DataAccess.Repositories;

public sealed class StoreProductRepository : IStoreProductRepository
{
    private readonly GroceryStoreDbContext db;

    public StoreProductRepository(GroceryStoreDbContext db)
    {
        this.db = db;
    }

    public Task<List<StoreProductServerModel>> GetStoreProductsForUser(Guid userId)
    {
        return db.StoreProducts.Where(x => x.UserId == userId).Select(x => x.Adapt<StoreProductServerModel>()).ToListAsync();
    }

    public async Task<Guid> AddProduct(StoreProductModel product, Guid userId)
    {
        StoreProductDbModel storeProduct = new() { Name = product.Name, UnitPrice = product.UnitPrice, UserId = userId };
        db.StoreProducts.Add(storeProduct);
        await db.SaveChangesAsync();
        return storeProduct.Id;
    }

    public Task DeleteAll(Guid userId)
    {
        db.StoreProducts.RemoveRange(db.StoreProducts.Where(x => x.UserId == userId));
        return db.SaveChangesAsync();
    }

    public async Task<Exception?> UpdatePrice(Guid productId, Guid userId, double price)
    {
        StoreProductDbModel? product = await db.StoreProducts.FindAsync(productId, userId);
        if (product is null)
        {
            return NotFoundException.ForType<StoreProductDbModel>();
        }

        if (product.UserId != userId)
        {
            return ForbiddenException.Instance;
        }

        product.UnitPrice = price;
        await db.SaveChangesAsync();
        return null;
    }
}
