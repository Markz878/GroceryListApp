namespace GroceryListHelper.Server.Filters;

public class GroupProductsAccessFilter : IEndpointFilter
{
    private readonly ICartGroupRepository cartGroupRepository;

    public GroupProductsAccessFilter(ICartGroupRepository cartGroupRepository)
    {
        this.cartGroupRepository = cartGroupRepository;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        Guid groupId = context.GetArgument<Guid>(0);
        string email = context.HttpContext.User.GetUserEmail();
        bool hasAccess = await cartGroupRepository.CheckGroupAccess(groupId, email);
        if (hasAccess)
        {
            return await next(context);
        }
        else
        {
            return TypedResults.Forbid();
        }
    }
}
