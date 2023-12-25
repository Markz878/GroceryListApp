using Microsoft.AspNetCore.Antiforgery;

namespace BlazeGag.Server.Filters;

public class AntiforgeryTokenFilter(IAntiforgery antiforgery) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (await antiforgery.IsRequestValidAsync(context.HttpContext))
        {
            return await next(context);
        }
        else
        {
            return TypedResults.BadRequest("Incorrect token");
        }
    }
}
