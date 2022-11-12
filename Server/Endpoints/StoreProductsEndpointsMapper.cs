namespace GroceryListHelper.Server.Endpoints;

public static class StoreProductsEndpointsMapper
{
    public static void AddStoreProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("storeproducts").RequireAuthorization();
        group.MapGet("", GetAll);
        group.MapPost("", AddProduct).AddEndpointFilterFactory(ValidatorFactory.Validator<StoreProductModel>);
        group.MapDelete("", DeleteAllProducts);
        group.MapPut("", UpdateProduct).AddEndpointFilterFactory(ValidatorFactory.Validator<StoreProductServerModel>);
    }

    private static async Task<IResult> GetAll(ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        List<StoreProductServerModel> results = await storeProductsRepository.GetStoreProductsForUser(user.GetUserId().GetValueOrDefault());
        return TypedResults.Ok(results);
    }

    private static async Task<IResult> AddProduct(StoreProductModel product, ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        Guid id = await storeProductsRepository.AddProduct(product, user.GetUserId().GetValueOrDefault());
        return TypedResults.Created($"api/storeProducts", id);
    }

    private static async Task<IResult> DeleteAllProducts(ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        await storeProductsRepository.DeleteAll(user.GetUserId().GetValueOrDefault());
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateProduct(StoreProductServerModel updatedProduct, ClaimsPrincipal user, IStoreProductRepository storeProductsRepository)
    {
        await storeProductsRepository.UpdatePrice(updatedProduct.Id, user.GetUserId().GetValueOrDefault(), updatedProduct.UnitPrice);
        return Results.NoContent();
    }
}
