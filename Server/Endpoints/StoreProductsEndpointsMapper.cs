using GroceryListHelper.DataAccess.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GroceryListHelper.Server.Endpoints;

public static class StoreProductsEndpointsMapper
{
    public static void AddStoreProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("storeproducts").RequireAuthorization().WithTags("Store Products");
        group.MapGet("", GetAll);
        group.MapPost("", AddProduct);
        group.MapDelete("", DeleteAllProducts);
        group.MapPut("", UpdateProduct);
    }

    public static async Task<Ok<List<StoreProductUIModel>>> GetAll(ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        List<StoreProductUIModel> results = await storeProductsRepository.GetStoreProductsForUser(user.GetUserId().GetValueOrDefault());
        return TypedResults.Ok(results);
    }

    public static async Task<Created<Guid>> AddProduct(StoreProduct product, ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        Guid id = await storeProductsRepository.AddProduct(product, user.GetUserId().GetValueOrDefault());
        return TypedResults.Created($"api/storeProducts", id);
    }

    public static async Task<NoContent> DeleteAllProducts(ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        await storeProductsRepository.DeleteAll(user.GetUserId().GetValueOrDefault());
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> UpdateProduct(StoreProductUIModel updatedProduct, ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        Exception? ex = await storeProductsRepository.UpdatePrice(updatedProduct.Id, user.GetUserId().GetValueOrDefault(), updatedProduct.UnitPrice);
        return ex switch
        {
            NotFoundException => TypedResults.NotFound(),
            ForbiddenException => TypedResults.Forbid(),
            null => TypedResults.NoContent(),
            _ => throw new UnreachableException()
        };
    }
}
