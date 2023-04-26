using GroceryListHelper.DataAccess.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GroceryListHelper.Server.Endpoints;

public static class StoreProductsEndpointsMapper
{
    public static void AddStoreProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("storeproducts").WithTags("Store Products");
        group.MapGet("", GetAll);
        group.MapPost("", AddProduct);
        group.MapDelete("", DeleteAllProducts);
        group.MapPut("", UpdateProduct);
    }

    public static async Task<Ok<List<StoreProduct>>> GetAll(ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        List<StoreProduct> results = await storeProductsRepository.GetStoreProductsForUser(user.GetUserId().GetValueOrDefault());
        return TypedResults.Ok(results);
    }

    public static async Task<Created> AddProduct(StoreProduct product, ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        await storeProductsRepository.AddProduct(product, user.GetUserId().GetValueOrDefault());
        return TypedResults.Created($"api/storeProducts");
    }

    public static async Task<NoContent> DeleteAllProducts(ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        await storeProductsRepository.DeleteAll(user.GetUserId().GetValueOrDefault());
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> UpdateProduct(StoreProduct updatedProduct, ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        NotFoundError? ex = await storeProductsRepository.UpdatePrice(updatedProduct.Name, user.GetUserId().GetValueOrDefault(), updatedProduct.UnitPrice);
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
