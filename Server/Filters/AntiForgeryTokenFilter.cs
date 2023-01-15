using Microsoft.AspNetCore.Antiforgery;

namespace GroceryListHelper.Server.Filters;

public sealed class AntiForgeryTokenFilter : IEndpointFilter
{
    private readonly IAntiforgery antiforgery;
    public AntiForgeryTokenFilter(IAntiforgery antiforgery)
    {
        this.antiforgery = antiforgery;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            if (context.HttpContext.Request.Method is not "GET" and not "HEAD")
            {
                await antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            return await next(context);
        }
        catch (AntiforgeryValidationException)
        {
            return Results.BadRequest("Required header missing.");
        }
    }
}
