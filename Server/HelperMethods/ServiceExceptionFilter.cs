namespace GroceryListHelper.Server.HelperMethods;

public class ServiceExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            NotFoundException => new NotFoundObjectResult(context.Exception.Message),
            ForbiddenException => new ForbidResult(),
            _ => null,
        };
    }
}
