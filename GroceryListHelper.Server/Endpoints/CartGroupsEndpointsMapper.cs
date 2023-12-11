using GroceryListHelper.Core.Features.CartGroups;
using GroceryListHelper.Shared.Models.HelperModels;

namespace GroceryListHelper.Server.Endpoints;

public static class CartGroupsEndpointsMapper
{
    public static void AddCartGroupEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("cartgroups")
            .WithTags("Cart Groups");

        group.MapGet("", GetGroupsForUser)
            .WithName("Get cart groups for user");

        group.MapGet("/{groupId:guid}", GetGroup)
            .AddEndpointFilter<GroupAccessFilter>()
            .WithName("Get cart group info");

        group.MapPost("", CreateGroup);

        group.MapDelete("/{groupId:guid}", DeleteCartGroup)
            .AddEndpointFilter<GroupAccessFilter>();

        group.MapPut("/{groupId:guid}", UpdateGroup)
            .AddEndpointFilter<GroupAccessFilter>();

    }

    public static async Task<Results<Ok<List<CartGroup>>, ForbidHttpResult>> GetGroupsForUser(ClaimsPrincipal user, IMediator mediator)
    {
        List<CartGroup> results = await mediator.Send(new GetUserCartGroupsQuery() { UserEmail = user.GetUserEmail() ?? "" });
        return TypedResults.Ok(results);
    }

    public static async Task<Results<Ok<CartGroup>, ForbidHttpResult, NotFound>> GetGroup(Guid groupId, ClaimsPrincipal user, IMediator mediator)
    {
        Result<CartGroup, ForbiddenError, NotFoundError> cartGroupResponse = await mediator.Send(new GetCartGroupQuery() { GroupId = groupId, UserEmail = user.GetUserEmail() ?? "" });
        return cartGroupResponse.Map<Results<Ok<CartGroup>, ForbidHttpResult, NotFound>>(
            x => TypedResults.Ok(x),
            e => TypedResults.Forbid(),
            e => TypedResults.NotFound());
    }

    public static async Task<Results<Created<Guid>, NotFound<string>>> CreateGroup(CreateCartGroupRequest request, ClaimsPrincipal user, IMediator mediator)
    {
        request.OtherUsers.Add(user.GetUserEmail() ?? "");
        Result<Guid, NotFoundError> createGroupResponse = await mediator.Send(new CreateGroupCommand() { GroupName = request.Name, UserEmails = request.OtherUsers });
        return createGroupResponse.Map<Results<Created<Guid>, NotFound<string>>>(
            x => TypedResults.Created($"api/cartgroups", x),
            e => TypedResults.NotFound(e.Message));
    }

    public static async Task<Results<NoContent, ForbidHttpResult, NotFound>> DeleteCartGroup(Guid groupId, ClaimsPrincipal user, IMediator mediator)
    {
        NotFoundError? ex = await mediator.Send(new DeleteCartGroupCommand() { GroupId = groupId, UserEmail = user.GetUserEmail() ?? "" });
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, ForbidHttpResult, NotFound>> UpdateGroup(Guid groupId, UpdateCartGroupNameRequest updatedGroup, ClaimsPrincipal user, IMediator mediator)
    {
        NotFoundError? ex = await mediator.Send(new UpdateGroupNameCommand() { GroupId = groupId, GroupName = updatedGroup.Name, UserEmail = user.GetUserEmail() ?? "" });
        return ex == null ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
