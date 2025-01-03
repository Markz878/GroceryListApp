using GroceryListHelper.Core.Domain.CartProducts;
using GroceryListHelper.Server.HelperMethods;
using System.ComponentModel;

namespace GroceryListHelper.Server.Endpoints;

public static class CartProductsEndpointsMapper
{
    public static void AddCartProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartproducts")
            .WithTags("Cart Products");

        group.MapGet("", GetAll)
            .WithName("Get cart products for a user");

        group.MapPost("", UpsertCartProduct)
            .WithDescription("Upsert cart products to a cart.");

        group.MapDelete("", DeleteAllProducts);

        group.MapDelete("/{productName}", DeleteProduct);

        group.MapPatch("/sort/{sortDirection:int:range(0,1)}", SortCartProducts);
    }

    public static async Task<Ok<List<CartProduct>>> GetAll(ClaimsPrincipal user, IMediator mediator)
    {
        List<CartProduct> results = await mediator.Send(new GetUserCartProductsQuery() { UserId = user.GetUserId() });
        return TypedResults.Ok(results);
    }

    public static async Task<NoContent> DeleteAllProducts(ClaimsPrincipal user, IMediator mediator)
    {
        await mediator.Send(new ClearUserCartProductsCommand() { UserId = user.GetUserId() });
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteProduct(string productName, ClaimsPrincipal user, IMediator mediator)
    {
        NotFoundError? ex = await mediator.Send(new DeleteCartProductCommand() { ProductName = productName, UserId = user.GetUserId() });
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<Created, NotFound, ForbidHttpResult, ProblemHttpResult>> UpsertCartProduct(CartProduct updatedProduct, ClaimsPrincipal user, IMediator mediator)
    {
        await mediator.Send(new UpsertCartProductsCommand() { UserId = user.GetUserId(), CartProducts = [updatedProduct] });
        return TypedResults.Created();
    }

    public static async Task<NoContent> SortCartProducts(ListSortDirection sortDirection, ClaimsPrincipal user, IMediator mediator)
    {
        await mediator.Send(new SortCartProductsByNameCommand() { UserId = user.GetUserId(), SortDirection = sortDirection });
        return TypedResults.NoContent();
    }
}
