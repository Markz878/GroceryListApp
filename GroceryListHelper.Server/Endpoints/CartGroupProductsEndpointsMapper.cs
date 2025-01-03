﻿using GroceryListHelper.Core.Domain.CartProducts;
using GroceryListHelper.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;

namespace GroceryListHelper.Server.Endpoints;

public static class CartGroupProductsEndpointsMapper
{
    public static void AddCartGroupProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartgroupproducts")
            .AddEndpointFilter<GroupAccessFilter>()
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

    public static async Task<Ok<List<CartProduct>>> GetAll(Guid groupId, IMediator mediator)
    {
        List<CartProduct> results = await mediator.Send(new GetUserCartProductsQuery() { UserId = groupId });
        return TypedResults.Ok(results);
    }

    public static async Task<Results<Created, Conflict>> AddProductToGroup(Guid groupId, [FromHeader] string? connectionId, CartProduct product, IMediator mediator, IHubContext<CartHub> hub)
    {
        await mediator.Send(new UpsertCartProductsCommand() { UserId = groupId, CartProducts = [product] });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductAdded), product);
        return TypedResults.Created($"api/cartgroupproducts/{groupId}");
    }

    public static async Task<NoContent> DeleteAllProducts(Guid groupId, [FromHeader] string? connectionId, IMediator mediator, IHubContext<CartHub> hub)
    {
        await mediator.Send(new ClearUserCartProductsCommand() { UserId = groupId });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductsDeleted));
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteProduct(Guid groupId, string productName, [FromHeader] string? connectionId, IMediator mediator, IHubContext<CartHub> hub)
    {
        NotFoundError? ex = await mediator.Send(new DeleteCartProductCommand() { UserId = groupId, ProductName = productName });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductDeleted), productName);
        return ex is null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> UpdateProduct(Guid groupId, CartProduct updatedProduct, [FromHeader] string? connectionId, IMediator mediator, IHubContext<CartHub> hub)
    {
        await mediator.Send(new UpsertCartProductsCommand() { UserId = groupId, CartProducts = [updatedProduct] });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductModified), updatedProduct);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> SortCartProducts(Guid groupId, ListSortDirection sortDirection, [FromHeader] string? connectionId, IMediator mediator, IHubContext<CartHub> hub)
    {
        await mediator.Send(new SortCartProductsByNameCommand() { UserId = groupId, SortDirection = sortDirection });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductsSorted), sortDirection);
        return TypedResults.NoContent();
    }
}
