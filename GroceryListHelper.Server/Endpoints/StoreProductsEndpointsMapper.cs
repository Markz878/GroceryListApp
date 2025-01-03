using GroceryListHelper.Core.Domain.StoreProducts;
using GroceryListHelper.Server.HelperMethods;

namespace GroceryListHelper.Server.Endpoints;

public static class StoreProductsEndpointsMapper
{
    public static void AddStoreProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("storeproducts").WithTags("Store Products");
        group.MapGet("", GetAll);
        group.MapPost("", UpsertProducts);
        group.MapDelete("", DeleteAllProducts);
    }

    public static async Task<Ok<List<StoreProduct>>> GetAll(ClaimsPrincipal user, IMediator mediator)
    {
        List<StoreProduct> results = await mediator.Send(new GetUserStoreProductsQuery() { UserId = user.GetUserId() });
        return TypedResults.Ok(results);
    }


    public static async Task<NoContent> DeleteAllProducts(ClaimsPrincipal user, IMediator mediator)
    {
        await mediator.Send(new DeleteAllUserStoreProductsCommand() { UserId = user.GetUserId() });
        return TypedResults.NoContent();
    }

    public static async Task<Results<Created, NotFound, ForbidHttpResult>> UpsertProducts(StoreProduct updatedProduct, ClaimsPrincipal user, IMediator mediator)
    {
        await mediator.Send(new UpsertStoreProductsCommand()
        {
            UserId = user.GetUserId(),
            StoreProducts = [updatedProduct]
        });
        return TypedResults.Created();
    }
}
