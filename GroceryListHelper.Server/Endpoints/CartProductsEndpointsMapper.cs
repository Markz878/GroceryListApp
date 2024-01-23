using GroceryListHelper.Core.Domain.CartProducts;
using GroceryListHelper.Server.HelperMethods;

namespace GroceryListHelper.Server.Endpoints;

public static class CartProductsEndpointsMapper
{
    public static void AddCartProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartproducts")
            .WithTags("Cart Products");

        group.MapGet("", GetAll)
            .WithName("Get cart products for a user");

        group.MapPost("", AddProduct)
            .WithSummary("Add a cart product")
            .WithDescription("Add a cart product to your cart.");

        group.MapDelete("", DeleteAllProducts);

        group.MapDelete("/{productName}", DeleteProduct);

        group.MapPut("", UpdateProduct);

        group.MapPatch("/sort/{sortDirection:int:range(0,1)}", SortCartProducts);
    }

    public static async Task<Ok<List<CartProductCollectable>>> GetAll(ClaimsPrincipal user, IMediator mediator)
    {
        List<CartProductCollectable> results = await mediator.Send(new GetUserCartProductsQuery() { UserId = user.GetUserId() });
        return TypedResults.Ok(results);
    }

    public static async Task<Results<Created, Conflict<string>>> AddProduct(CartProduct product, ClaimsPrincipal user, IMediator mediator)
    {
        ConflictError? error = await mediator.Send(new AddCartProductCommand() { CartProduct = product, UserId = user.GetUserId() });
        return error is null ? TypedResults.Created($"api/cartproducts") : TypedResults.Conflict("Product already exists");
    }

    public static async Task<NoContent> DeleteAllProducts(ClaimsPrincipal user, IMediator mediator)
    {
        await mediator.Send(new ClearCartProductsCommand() { UserId = user.GetUserId() });
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteProduct(string productName, ClaimsPrincipal user, IMediator mediator)
    {
        NotFoundError? ex = await mediator.Send(new DeleteCartProductCommand() { ProductName = productName, UserId = user.GetUserId() });
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult, ProblemHttpResult>> UpdateProduct(CartProductCollectable updatedProduct, ClaimsPrincipal user, IMediator mediator)
    {
        NotFoundError? ex = await mediator.Send(new UpdateProductCommand() { UserId = user.GetUserId(), CartProduct = updatedProduct });
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<NoContent> SortCartProducts(ListSortDirection sortDirection, ClaimsPrincipal user, IMediator mediator)
    {
        await mediator.Send(new SortCartProductsCommand() { UserId = user.GetUserId(), SortDirection = sortDirection });
        return TypedResults.NoContent();
    }
}
