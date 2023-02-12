using GroceryListHelper.Server.Filters;

namespace GroceryListHelper.Server.Endpoints;

public static class APIEndpointMapper
{
    public static void MapAPIEndpoints(this WebApplication app)
    {
        RouteGroupBuilder apiGroup = app.MapGroup("api").AddFluentValidation();
        if (app.Environment.IsProduction())
        {
            apiGroup.RequireRateLimiting(RateLimitInstaller.PolicyName);
            apiGroup.AddEndpointFilter<AntiForgeryTokenFilter>();
        }
        apiGroup.AddAccountEndpoints();
        apiGroup.AddCartProductEndpoints();
        apiGroup.AddStoreProductEndpoints();
    }
}
