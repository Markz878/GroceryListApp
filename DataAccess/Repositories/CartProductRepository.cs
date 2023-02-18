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

        //AsyncPageable<CartProductDbModel> cartProductPages = db.QueryAsync<CartProductDbModel>(x => x.PartitionKey == ownerId.ToString());
        //List<CartProductCollectable> result = new();
        //string? token = null;
        //await foreach (Page<CartProductDbModel> cartProductPage in cartProductPages.AsPages())
        //{
        //    token = cartProductPage.ContinuationToken;
        //    result.AddRange(cartProductPage.Values.Select(x => new CartProductCollectable()
        //    {
        //        Amount = x.Amount,
        //        IsCollected = x.IsCollected,
        //        Name = x.Name,
        //        Order = x.Order,
        //        UnitPrice = x.UnitPrice
        //    }));
        //}
        return products.Select(x => new CartProductCollectable() { Amount = x.Amount, IsCollected = x.IsCollected, Name = x.Name, Order = x.Order, UnitPrice = x.UnitPrice }).ToList();
    }

    public async Task ClearCartProducts(Guid ownerId)
    {
        //AsyncPageable<CartProductDbModel> cartProductPages = db.QueryAsync<CartProductDbModel>(x => x.PartitionKey == ownerId.ToString());
        //string? token = null;
        //List<CartProductDbModel> products = new();
        //await foreach (Page<CartProductDbModel> cartProductPage in cartProductPages.AsPages(token))
        //{
        //    token = cartProductPage.ContinuationToken;
        //    products.AddRange(cartProductPage.Values);
        //}
        List<CartProductDbModel> products = await db.GetTableEntitiesByPrimaryKey<CartProductDbModel>(ownerId.ToString());
        Response<IReadOnlyList<Response>> response = await db.SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
    }

    public async Task<Exception?> DeleteProduct(string productName, Guid ownerId)
    {
        Response response = await db.DeleteEntityAsync(ownerId.ToString(), productName);
        return response.Status == 404 ? NotFoundException.ForType<CartProduct>() : null;
    }

    public async Task<Exception?> UpdateProduct(Guid ownerId, CartProductCollectable updatedProduct)
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
            return NotFoundException.ForType<CartProduct>();
        }
    }

    public async Task SortUserProducts(Guid ownerId, ListSortDirection sortDirection)
    {
        List<CartProductDbModel> products = await db.GetTableEntitiesByPrimaryKey<CartProductDbModel>(ownerId.ToString());
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
        Response<IReadOnlyList<Response>> response = await db.SubmitTransactionAsync(products.Select(x => new TableTransactionAction(TableTransactionActionType.UpdateMerge, x)));
    }
}
