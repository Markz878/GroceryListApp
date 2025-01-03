using Microsoft.Azure.Cosmos;

namespace GroceryListHelper.Server.Filters;

public class ExceptionFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
		try
		{
			return await next(context);
        }
		catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
		{
			return TypedResults.NotFound();
        }
    }
}
