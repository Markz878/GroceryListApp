using GroceryListHelper.DataAccess.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;

namespace GroceryListHelper.Server.Endpoints;

public static class CartProductsEndpointsMapper
{
    public static void AddCartProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartproducts").RequireAuthorization().WithTags("Cart Products");
        group.MapGet("", GetAll).WithName("Get cart products");
        group.MapPost("", AddProduct)
            .WithSummary("Add a cart product")
            .WithDescription("Add a cart product to your cart.");
        group.MapDelete("", DeleteAllProducts);
        group.MapDelete("/{id:guid}", DeleteProduct);
        group.MapPut("", UpdateProduct);
        group.MapPatch("/sort/{sortDirection:int:range(0,1)}", SortCartProducts);
    }

    public static async Task<Ok<List<CartProductCollectable>>> GetAll(ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        List<CartProductCollectable> results = await cartProductsRepository.GetCartProductsForUser(user.GetUserId().GetValueOrDefault());
        return TypedResults.Ok(results);
    }

    public static async Task<Created<Guid>> AddProduct(CartProduct product, ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        Guid id = await cartProductsRepository.AddCartProduct(product, user.GetUserId().GetValueOrDefault());
        return TypedResults.Created($"api/cartproducts", id);
    }

    public static async Task<NoContent> DeleteAllProducts(ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        await cartProductsRepository.ClearProductsForUser(user.GetUserId().GetValueOrDefault());
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteProduct(Guid id, ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        Exception? ex = await cartProductsRepository.DeleteProduct(id, user.GetUserId().GetValueOrDefault());
        return ex switch
        {
            NotFoundException => TypedResults.NotFound(),
            ForbiddenException => TypedResults.Forbid(),
            null => TypedResults.NoContent(),
            _ => throw new UnreachableException()
        };
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> UpdateProduct(CartProductCollectable updatedProduct, ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        Exception? ex = await cartProductsRepository.UpdateProduct(user.GetUserId().GetValueOrDefault(), updatedProduct);
        return ex switch
        {
            NotFoundException => TypedResults.NotFound(),
            ForbiddenException => TypedResults.Forbid(),
            null => TypedResults.NoContent(),
            _ => throw new UnreachableException()
        };
    }

    public static async Task<NoContent> SortCartProducts(ListSortDirection sortDirection, ClaimsPrincipal user, ICartProductRepository cartProductsRepository)
    {
        await cartProductsRepository.SortUserProducts(user.GetUserId().GetValueOrDefault(), sortDirection);
        return TypedResults.NoContent();
    }
}
