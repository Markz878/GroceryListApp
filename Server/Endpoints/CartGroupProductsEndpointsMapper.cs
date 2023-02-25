using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Server.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;

namespace GroceryListHelper.Server.Endpoints;

public static class CartGroupProductsEndpointsMapper
{
    public static void AddCartGroupProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartgroupproducts")
            .RequireAuthorization()
            .AddEndpointFilter<GroupProductsAccessFilter>()
            .WithTags("Cart Group Products");

        group.MapGet("/{groupId:guid}", GetAll)
            .WithName("Get cart products for a group");

        group.MapPost("/{groupId:guid}", AddProductToGroup)
            .WithSummary("Add a cart product to a group")
            .WithDescription("Add a cart product to a group cart.");

        group.MapDelete("/{groupId:guid}", DeleteAllProducts);

        group.MapDelete("/{groupId:guid}/{productName}", DeleteProduct);

        group.MapPut("/{groupId:guid}", UpdateProduct);

        group.MapPatch("/{groupId:guid}/sort/{sortDirection:int:range(0,1)}", SortCartProducts);
    }

    public static async Task<Ok<List<CartProductCollectable>>> GetAll(Guid groupId, ICartProductRepository cartProductsRepository, ICartGroupRepository cartGroupRepository)
    {
        List<CartProductCollectable> results = await cartProductsRepository.GetCartProducts(groupId);
        return TypedResults.Ok(results);
    }

    public static async Task<Created> AddProductToGroup(Guid groupId, CartProduct product, ICartProductRepository cartProductsRepository)
    {
        await cartProductsRepository.AddCartProduct(product, groupId);
        return TypedResults.Created($"api/cartgroupproducts/{groupId}");
    }

    public static async Task<NoContent> DeleteAllProducts(Guid groupId, ICartProductRepository cartProductsRepository)
    {
        await cartProductsRepository.ClearCartProducts(groupId);
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, Microsoft.AspNetCore.Http.HttpResults.NotFound, ForbidHttpResult>> DeleteProduct(Guid groupId, string productName, ICartProductRepository cartProductsRepository)
    {
        DataAccess.Exceptions.NotFoundError? ex = await cartProductsRepository.DeleteProduct(productName, groupId);
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, Microsoft.AspNetCore.Http.HttpResults.NotFound, ForbidHttpResult>> UpdateProduct(Guid groupId, CartProductCollectable updatedProduct, ICartProductRepository cartProductsRepository)
    {
        DataAccess.Exceptions.NotFoundError? ex = await cartProductsRepository.UpdateProduct(groupId, updatedProduct);
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<NoContent> SortCartProducts(Guid groupId, ListSortDirection sortDirection, ICartProductRepository cartProductsRepository)
    {
        await cartProductsRepository.SortCartProducts(groupId, sortDirection);
        return TypedResults.NoContent();
    }
}
