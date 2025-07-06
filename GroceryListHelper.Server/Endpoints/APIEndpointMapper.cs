namespace GroceryListHelper.Server.Endpoints;

public static class APIEndpointMapper
{
    public static void MapAPIEndpoints(this WebApplication app)
    {
        RouteGroupBuilder apiGroup = app.MapGroup("api").RequireAuthorization();
        apiGroup.AddFluentValidation();
        apiGroup.RequireRateLimiting(RateLimitInstaller.PolicyName);
        apiGroup.AddEndpointFilter<ExceptionFilter>();
        apiGroup.AddAccountEndpoints();
        apiGroup.AddCartProductEndpoints();
        apiGroup.AddCartGroupProductEndpoints();
        apiGroup.AddStoreProductEndpoints();
        apiGroup.AddCartGroupEndpoints();
    }
}
