using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared;
using GroceryListHelper.Shared.Models.CartProduct;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GroceryListHelper.DataAccess.Repositories;

public class CartProductRepository : ICartProductRepository
{
    private readonly GroceryStoreDbContext db;
    public CartProductRepository(GroceryStoreDbContext db)
    {
        this.db = db;
    }

    public async Task<string> AddCartProduct(CartProduct cartProduct, string userId)
    {
        CartProductDbModel cartDbProduct = cartProduct.Adapt<CartProductDbModel>();
        cartDbProduct.UserId = userId;
        db.CartProducts.Add(cartDbProduct);
        await db.SaveChangesAsync();
        return cartDbProduct.Id;
    }

    public Task<List<CartProductCollectable>> GetCartProductsForUser(string userId)
    {
        return db.CartProducts.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new CartProductCollectable() { Amount = x.Amount, IsCollected = x.IsCollected, Name = x.Name, Id = x.Id, UnitPrice = x.UnitPrice })
            .ToListAsync();
    }

    public async Task<CartProductCollectable> GetCartProductForUser(string productId, string userId)
    {
        CartProductDbModel cartProduct = await db.CartProducts
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == productId && x.UserId == userId);
        return cartProduct.Adapt<CartProductCollectable>();
    }

    public Task RemoveItemsForUser(string userId)
    {
        db.CartProducts.RemoveRange(db.CartProducts.Where(x => x.UserId == userId));
        return db.SaveChangesAsync();
    }

    public async Task<bool> DeleteItem(string productId, string userId)
    {
        CartProductDbModel product = await db.CartProducts.FindAsync(productId, userId);
        if (product == null || product.UserId != userId)
        {
            return false;
        }
        db.CartProducts.Remove(product);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsCollected(string productId, string userId)
    {
        CartProductDbModel product = await db.CartProducts.FindAsync(productId, userId);
        if (product == null || product.UserId != userId)
        {
            return false;
        }
        product.IsCollected ^= true;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProduct(string productId, string userId, CartProduct updatedProduct)
    {
        CartProductDbModel product = await db.CartProducts.FindAsync(productId, userId);
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
