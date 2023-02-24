using GroceryListHelper.DataAccess.Exceptions;
using GroceryListHelper.Shared.Models.HelperModels;
using Microsoft.AspNetCore.Http.HttpResults;

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

        group.MapPost("", CreateGroup)
            .WithSummary("Add a cart product")
            .WithDescription("Add a cart product to your cart.");

        group.MapDelete("/{groupId:guid}", DeleteCartGroup);

        group.MapPut("", UpdateGroup);
    }

    public static async Task<Results<Ok<List<CartGroup>>, ForbidHttpResult>> GetGroupsForUser(ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        List<CartGroup> results = await cartGroupRepository.GetCartGroupsForUser(user.GetUserEmail());
        return TypedResults.Ok(results);
    }

    public static async Task<Results<Ok<CartGroup>, ForbidHttpResult, NotFound<string>>> GetGroup(Guid groupId, ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        Response<CartGroup, Forbidden, DataAccess.Exceptions.NotFound> cartGroupResponse = await cartGroupRepository.GetCartGroup(groupId, user.GetUserEmail());
        return cartGroupResponse.Match<Results<Ok<CartGroup>, ForbidHttpResult, NotFound<string>>>(
            x => TypedResults.Ok(x), 
            e => TypedResults.Forbid(), 
            e => TypedResults.NotFound(e.Message));
    }
    public static async Task<Results<Created<Guid>, NotFound<string>>> CreateGroup(CreateCartGroupRequest request, ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        request.OtherUsers.Add(user.GetUserEmail());
        Response<Guid, DataAccess.Exceptions.NotFound> createGroupResponse = await cartGroupRepository.CreateGroup(request.Name, request.OtherUsers);
        return createGroupResponse.Match<Results<Created<Guid>, NotFound<string>>>(
            x=> TypedResults.Created($"api/cartgroups", x), 
            e=> TypedResults.NotFound(e.Message));
    }

    public static async Task<Results<NoContent, ForbidHttpResult, NotFound<string>>> DeleteCartGroup(Guid groupId, ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        DataAccess.Exceptions.NotFound? ex = await cartGroupRepository.DeleteCartGroup(groupId, user.GetUserEmail());
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound(ex.Value.Message);
    }

    public static async Task<Results<NoContent, ForbidHttpResult, NotFound<string>>> UpdateGroup(UpdateCartGroupNameRequest updatedGroup, ClaimsPrincipal user, ICartGroupRepository cartGroupRepository)
    {
        DataAccess.Exceptions.NotFound? ex = await cartGroupRepository.UpdateGroupName(updatedGroup.GroupId, updatedGroup.Name, user.GetUserEmail());
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound(ex.Value.Message);
    }
}
