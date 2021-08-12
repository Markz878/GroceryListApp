using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.DataAccess.Repositories
{
    public class CartProductRepository : ICartProductRepository
    {
        private readonly GroceryStoreDbContext db;
        public CartProductRepository(GroceryStoreDbContext db)
        {
            this.db = db;
        }

        public async Task<int> AddCartProduct(CartProduct cartProduct, int userId)
        {
            CartProductDbModel cartDbProduct = new() { Amount = cartProduct.Amount, Name = cartProduct.Name, UnitPrice = cartProduct.UnitPrice, UserId = userId };
            db.CartProducts.Add(cartDbProduct);
            await db.SaveChangesAsync();
            return cartDbProduct.Id;
        }

        public async Task<IEnumerable<CartProductCollectable>> GetCartProductsForUser(int userId)
        {
            return (await db.CartProducts.Where(x => x.UserId == userId).AsNoTracking().ToListAsync()).ConvertAll(x => (CartProductCollectable)x);
        }

        public Task RemoveItemsForUser(int userId)
        {
            db.CartProducts.RemoveRange(db.CartProducts.Where(x => x.UserId == userId));
            return db.SaveChangesAsync();
        }

        public async Task<bool> DeleteItem(int productId, int userId)
        {
            CartProductDbModel product = await db.CartProducts.FindAsync(productId);
            if (product == null || product.UserId != userId)
            {
                return false;
            }
            db.CartProducts.Remove(product);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsCollected(int productId, int userId)
        {
            CartProductDbModel product = await db.CartProducts.FindAsync(productId);
            if (product == null || product.UserId != userId)
            {
                return false;
            }
            product.IsCollected ^= true;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProduct(int productId, int userId, CartProduct updatedProduct)
        {
            CartProductDbModel product = await db.CartProducts.FindAsync(productId);
            if (product == null || product.UserId != userId)
            {
                return false;
            }
            product.Name = updatedProduct.Name;
            product.Amount = updatedProduct.Amount;
            product.UnitPrice = updatedProduct.UnitPrice;
            await db.SaveChangesAsync();
            return true;
        }
    }
}
