using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.DataAccess.Repositories
{
    public class StoreProductRepository : IStoreProductRepository
    {
        private readonly GroceryStoreDbContext db;

        public StoreProductRepository(GroceryStoreDbContext db)
        {
            this.db = db;
        }

        public IAsyncEnumerable<StoreProductDbModel> GetStoreProductsForUser(int userId)
        {
            return db.StoreProducts.Where(x => x.UserId == userId).AsAsyncEnumerable();
        }

        public async Task<int> AddProduct(StoreProduct product, int userId)
        {
            StoreProductDbModel storeProduct = new() { Name = product.Name, UnitPrice = product.UnitPrice, UserId = userId };
            db.StoreProducts.Add(storeProduct);
            await db.SaveChangesAsync();
            return storeProduct.Id;
        }

        public Task DeleteAll(int userId)
        {
            db.StoreProducts.RemoveRange(db.StoreProducts.Where(x => x.UserId == userId));
            return db.SaveChangesAsync();
        }

        public async Task<bool> DeleteItem(int productId, int userId)
        {
            StoreProductDbModel product = db.StoreProducts.Find(productId);
            if (product == null || product.UserId != userId)
            {
                return false;
            }
            db.StoreProducts.Remove(product);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePrice(int productId, int userId, double price)
        {
            StoreProductDbModel product = await db.StoreProducts.FindAsync(productId);
            if (product == null || product.UserId != userId)
            {
                return false;
            }
            product.UnitPrice = price;
            await db.SaveChangesAsync();
            return true;
        }
    }
}
