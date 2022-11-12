namespace GroceryListHelper.Server.Endpoints;

public static class APIEndpointMapper
{
    public static void MapAPIEndpoints(this WebApplication app)
    {
        RouteGroupBuilder apiGroup = app.MapGroup("api").RequireRateLimiting(RateLimitInstaller.PolicyName).WithOpenApi();
        if (app.Environment.IsProduction())
        {
            apiGroup.AddEndpointFilter<AntiForgeryTokenFilter>();
        }
        apiGroup.AddAccountEndpoints();
        apiGroup.AddUserEndpoints();
        apiGroup.AddCartProductEndpoints();
        apiGroup.AddStoreProductEndpoints();
    }
}
