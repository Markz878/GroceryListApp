using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared.Models.CartProduct;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GroceryListHelper.DataAccess.Repositories;

public sealed class CartProductRepository : ICartProductRepository
{
    private readonly GroceryStoreDbContext db;
    public CartProductRepository(GroceryStoreDbContext db)
    {
        this.db = db;
    }

    public async Task<Guid> AddCartProduct(CartProduct cartProduct, Guid userId)
    {
        CartProductDbModel cartDbProduct = cartProduct.Adapt<CartProductDbModel>();
        cartDbProduct.UserId = userId;
        db.CartProducts.Add(cartDbProduct);
        await db.SaveChangesAsync();
        return cartDbProduct.Id;
    }

    public Task<List<CartProductCollectable>> GetCartProductsForUser(Guid userId)
    {
        return db.CartProducts
            .Where(x => x.UserId == userId)
            .Select(x => new CartProductCollectable() { Amount = x.Amount, IsCollected = x.IsCollected, Name = x.Name, Id = x.Id, UnitPrice = x.UnitPrice, Order = x.Order })
            .ToListAsync();
    }

    public Task ClearProductsForUser(Guid userId)
    {
        db.CartProducts.RemoveRange(db.CartProducts.Where(x => x.UserId == userId));
        return db.SaveChangesAsync();
    }

    public async Task<Exception?> DeleteProduct(Guid productId, Guid userId)
    {
        CartProductDbModel? product = await db.CartProducts.FindAsync(productId, userId);
        if (product is null)
        {
            return NotFoundException.ForType<CartProduct>();
        }
        if (product.UserId != userId)
        {
            return ForbiddenException.Instance;
        }
        db.CartProducts.Remove(product);
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<Exception?> UpdateProduct(Guid userId, CartProductCollectable updatedProduct)
    {
        CartProductDbModel? product = await db.CartProducts.FindAsync(updatedProduct.Id, userId);
        if (product is null)
        {
            return NotFoundException.ForType<CartProduct>();
        }
        if (product.UserId != userId)
        {
            return ForbiddenException.Instance;
        }
        updatedProduct.Adapt(product);
        await db.SaveChangesAsync();
        return null;
    }
}
