using Microsoft.AspNetCore.Antiforgery;

namespace GroceryListHelper.Server.Endpoints;

public class AntiForgeryTokenFilter : IEndpointFilter
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
            if (context.HttpContext.Request.Method is "POST" or "PUT" or "DELETE")
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
