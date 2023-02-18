using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Server.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;

namespace GroceryListHelper.Server.Endpoints;

public static class CartGroupsEndpointsMapper
{
    public static void AddCartGroupEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartgroups")
            .RequireAuthorization()
            .WithTags("Cart Groups");

        group.MapGet("", GetGroupsForUser)
            .WithName("Get cart groups for user");

        group.MapGet("/{groupId:guid}", GetGroup)
            .WithName("Get cart group info");

        group.MapPost("", AddGroup)
            .WithSummary("Add a cart product")
            .WithDescription("Add a cart product to your cart.");

        group.MapDelete("/{groupId:guid}", RemoveUserFromGroup);

        group.MapPut("", UpdateGroup);
    }

    public static async Task<Ok<List<CartGroup>>> GetGroupsForUser(ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        List<CartGroup> results = await cartGroupRepository.GetCartGroupsForUser(user.GetUserEmail());
        return TypedResults.Ok(results);
    }

    public static async Task<Results<Ok<CartGroup>, NotFound>> GetGroup(Guid groupId, ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        CartGroup? result = await cartGroupRepository.GetCartGroup(groupId, user.GetUserEmail());
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static async Task<Created<Guid>> AddGroup(CreateCartGroupRequest request, ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        string? userEmail = user.GetUserEmail();
        ArgumentNullException.ThrowIfNull(userEmail);
        request.OtherUsers.Add(userEmail);
        Guid id = await cartGroupRepository.CreateGroup(request.Name, request.OtherUsers);
        return TypedResults.Created($"api/cartgroups", id);
    }

    public static async Task<NoContent> RemoveUserFromGroup(Guid groupId, ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        await cartGroupRepository.DeleteCartGroup(groupId, user.GetUserEmail());
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> UpdateGroup(CartGroup updatedGroup, ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        bool hasAccess = await cartGroupRepository.CheckGroupAccess(updatedGroup.Id, user.GetUserEmail());
        if (hasAccess is false)
        {
            return TypedResults.Forbid();
        }
        Exception? ex = await cartGroupRepository.UpdateGroupName(updatedGroup.Id, updatedGroup.Name);
        return ex switch
        {
            NotFoundException => TypedResults.NotFound(),
            null => TypedResults.NoContent(),
            _ => throw new UnreachableException()
        };
    }
}
