namespace GroceryListHelper.Server.Endpoints;

public static class APIEndpointMapper
{
    public static void MapAPIEndpoints(this WebApplication app)
    {
        RouteGroupBuilder apiGroup = app.MapGroup("api").RequireAuthorization();
        if (app.Environment.IsProduction())
        {
            apiGroup.AddEndpointFilter<AntiforgeryTokenFilter>();
        }
        apiGroup.AddFluentValidation();
        apiGroup.RequireRateLimiting(RateLimitInstaller.PolicyName);
        apiGroup.AddAccountEndpoints();
        apiGroup.AddCartProductEndpoints();
        apiGroup.AddCartGroupProductEndpoints();
        apiGroup.AddStoreProductEndpoints();
        apiGroup.AddCartGroupEndpoints();
    }
}
