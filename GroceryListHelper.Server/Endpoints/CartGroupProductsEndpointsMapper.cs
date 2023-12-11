using GroceryListHelper.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GroceryListHelper.Server.Endpoints;

public static class CartGroupProductsEndpointsMapper
{
    public static void AddCartGroupProductEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartgroupproducts")
            .AddEndpointFilter<GroupAccessFilter>()
            .WithTags("Cart Group Products");

        //group.MapPost("/join/{groupId:guid}", JoinCartGroup)
        //    .AddEndpointFilter<GroupAccessFilter>();

        //group.MapPost("/leave/{groupId:guid}", LeaveCartGroup)
        //    .AddEndpointFilter<GroupAccessFilter>();

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


    //public static async Task<Results<NoContent, UnauthorizedHttpResult>> JoinCartGroup(Guid groupId, [FromHeader] string connectionId, ClaimsPrincipal user, IHubContext<CartHub> hub)
    //{
    //    await hub.Groups.AddToGroupAsync(connectionId, groupId.ToString());
    //    await hub.Clients.GroupExcept(groupId.ToString(), connectionId).SendAsync(nameof(ICartHubNotifications.GetMessage), $"User {user.GetUserEmail()} joined sharing");
    //    return TypedResults.NoContent();
    //}

    //public static async Task<Results<NoContent, UnauthorizedHttpResult>> LeaveCartGroup(Guid groupId, [FromHeader] string connectionId, ClaimsPrincipal user, IHubContext<CartHub> hub)
    //{
    //    await hub.Groups.RemoveFromGroupAsync(connectionId, groupId.ToString());
    //    await hub.Clients.GroupExcept(groupId.ToString(), connectionId).SendAsync(nameof(ICartHubNotifications.GetMessage), $"User {user.GetUserEmail()} left sharing");
    //    return TypedResults.NoContent();
    //}

    public static async Task<Ok<List<CartProductCollectable>>> GetAll(Guid groupId, IMediator mediator)
    {
        List<CartProductCollectable> results = await mediator.Send(new GetUserCartProductsQuery() { UserId = groupId });
        return TypedResults.Ok(results);
    }

    public static async Task<Created> AddProductToGroup(Guid groupId, [FromHeader] string? connectionId, CartProduct product, IMediator mediator, IHubContext<CartHub> hub)
    {
        await mediator.Send(new AddCartProductCommand() { UserId = groupId, CartProduct = product });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductAdded), product);
        return TypedResults.Created($"api/cartgroupproducts/{groupId}");
    }

    public static async Task<NoContent> DeleteAllProducts(Guid groupId, [FromHeader] string? connectionId, IMediator mediator, IHubContext<CartHub> hub)
    {
        await mediator.Send(new ClearCartProductsCommand() { UserId = groupId });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductsDeleted));
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteProduct(Guid groupId, string productName, [FromHeader] string? connectionId, IMediator mediator, IHubContext<CartHub> hub)
    {
        NotFoundError? ex = await mediator.Send(new DeleteCartProductCommand() { UserId = groupId, ProductName = productName });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductDeleted), productName);
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> UpdateProduct(Guid groupId, CartProductCollectable updatedProduct, [FromHeader] string? connectionId, IMediator mediator, IHubContext<CartHub> hub)
    {
        NotFoundError? ex = await mediator.Send(new UpdateProductCommand() { UserId = groupId, CartProduct = updatedProduct });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductModified), updatedProduct);
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<NoContent> SortCartProducts(Guid groupId, ListSortDirection sortDirection, [FromHeader] string? connectionId, IMediator mediator, IHubContext<CartHub> hub)
    {
        await mediator.Send(new SortCartProductsCommand() { UserId = groupId, SortDirection = sortDirection });
        await hub.Clients.GroupExcept(groupId.ToString(), connectionId ?? "").SendAsync(nameof(ICartHubNotifications.ProductsSorted), sortDirection);
        return TypedResults.NoContent();
    }
}
