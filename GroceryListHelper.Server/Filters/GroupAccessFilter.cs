using GroceryListHelper.Core.Features.CartGroups;

namespace GroceryListHelper.Server.Filters;

public class GroupAccessFilter(IMediator mediator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        Guid groupId = context.GetArgument<Guid>(0);
        string? email = context.HttpContext.User.GetDisplayName();
        if (string.IsNullOrWhiteSpace(email))
        {
            return TypedResults.Unauthorized();
        }
        bool hasAccess = await mediator.Send(new CheckGroupAccessQuery() { GroupId = groupId, UserEmail = email });
        if (hasAccess)
        {
            return await next(context);
        }
        else
        {
            return TypedResults.Unauthorized();
        }
    }
}
