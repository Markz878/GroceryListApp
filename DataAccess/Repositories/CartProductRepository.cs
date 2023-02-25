using Azure;
using Azure.Data.Tables;
using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.Shared.Models.CartProducts;
using System.ComponentModel;

namespace GroceryListHelper.DataAccess.Repositories;

public sealed class CartProductRepository : ICartProductRepository
{
    private readonly TableClient db;
    public CartProductRepository(TableServiceClient db)
    {
        this.db = db.GetTableClient(CartProductDbModel.GetTableName());
    }

    public async Task AddCartProduct(CartProduct cartProduct, Guid userId)
    {
        CartProductDbModel cartDbProduct = new()
        {
            Name = cartProduct.Name,
            Order = cartProduct.Order,
            OwnerId = userId,
            Amount = cartProduct.Amount,
            UnitPrice = cartProduct.UnitPrice
        };
        await db.AddEntityAsync(cartDbProduct);
    }

    public async Task<List<CartProductCollectable>> GetCartProducts(Guid ownerId)
    {
        List<CartProductDbModel> products = await db.GetTableEntitiesByPrimaryKey<CartProductDbModel>(ownerId.ToString());
        return products.Select(x => new CartProductCollectable() { Amount = x.Amount, IsCollected = x.IsCollected, Name = x.Name, Order = x.Order, UnitPrice = x.UnitPrice }).ToList();
    }

    public async Task ClearCartProducts(Guid ownerId)
    {
        List<CartProductDbModel> products = await db.GetTableEntitiesByPrimaryKey<CartProductDbModel>(ownerId.ToString());
        if (products.Count > 0)
        {
            await db.SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
        }
    }

    public async Task<NotFoundError?> DeleteProduct(string productName, Guid ownerId)
    {
        Response response = await db.DeleteEntityAsync(ownerId.ToString(), productName);
        return response.Status == 404 ? new NotFoundError() : null;
    }

    public async Task<NotFoundError?> UpdateProduct(Guid ownerId, CartProductCollectable updatedProduct)
    {
        CartProductDbModel dbProduct = new()
        {
            Amount = updatedProduct.Amount,
            IsCollected = updatedProduct.IsCollected,
            Name = updatedProduct.Name,
            Order = updatedProduct.Order,
            OwnerId = ownerId,
            UnitPrice = updatedProduct.UnitPrice
        };
        try
        {
            Response response = await db.UpdateEntityAsync(dbProduct, ETag.All, TableUpdateMode.Merge);
            return null;
        }
        catch (RequestFailedException ex) when (ex.Status is 404)
        {
            return new NotFoundError();
        }
    }

    public async Task SortCartProducts(Guid ownerId, ListSortDirection sortDirection)
    {
        List<CartProductDbModel> products = await db.GetTableEntitiesByPrimaryKey<CartProductDbModel>(ownerId.ToString());
        if (products.Count > 1)
        {
            int order = 1000;
            if (sortDirection == ListSortDirection.Ascending)
            {
                foreach (CartProductDbModel item in products.OrderBy(x => x.Name))
                {
                    item.Order = order;
                    order += 1000;
                }
            }
            else
            {
                foreach (CartProductDbModel item in products.OrderByDescending(x => x.Name))
                {
                    item.Order = order;
                    order += 1000;
                }
            }
            await db.SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.UpdateMerge, x)));
        }
    }
}
