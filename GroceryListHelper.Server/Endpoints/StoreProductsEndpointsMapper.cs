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

    public static async Task<Ok<List<StoreProduct>>> GetAll(ClaimsPrincipal user, IMediator mediator)
    {
        List<StoreProduct> results = await mediator.Send(new GetUserStoreProductsQuery() { UserId = user.GetUserId() });
        return TypedResults.Ok(results);
    }

    public static async Task<Results<Created, Conflict<string>>> AddProduct(StoreProduct product, ClaimsPrincipal user, IMediator mediator)
    {
        ConflictError? conflict = await mediator.Send(new AddStoreProductCommand() { Product = product, UserId = user.GetUserId() });
        return conflict is null ? TypedResults.Created($"api/storeProducts") : TypedResults.Conflict("Product already exists");
    }

    public static async Task<NoContent> DeleteAllProducts(ClaimsPrincipal user, IMediator mediator)
    {
        await mediator.Send(new DeleteAllUserStoreProductsCommand() { UserId = user.GetUserId() });
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> UpdateProduct(StoreProduct updatedProduct, ClaimsPrincipal user, IMediator mediator)
    {
        NotFoundError? ex = await mediator.Send(new UpdateStoreProductPriceCommand()
        {
            ProductName = updatedProduct.Name,
            UserId = user.GetUserId(),
            Price = updatedProduct.UnitPrice
        });
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
