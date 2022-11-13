namespace GroceryListHelper.Server.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> validator;

    public ValidationFilter(IValidator<T> validator)
    {
        this.validator = validator;
    }

    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.FirstOrDefault(x => x?.GetType() == typeof(T)) is not T parameter)
        {
            throw new ArgumentException($"No parameter of type {typeof(T).Name} given in endpoint {context.HttpContext.Request.Path}.");
        }
        ValidationResult validationResult = validator.Validate(parameter);
        if (!validationResult.IsValid)
        {
            return ValueTask.FromResult<object?>(Results.ValidationProblem(validationResult.ToDictionary()));
        }
        return next(context);
    }
}

public static class ValidatorFactory
{
    public static EndpointFilterDelegate Validator<T>(EndpointFilterFactoryContext handlerContext, EndpointFilterDelegate next)
    {
        int parameterIndex = -1;
        var parameters = handlerContext.MethodInfo.GetParameters();
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].ParameterType == typeof(T))
            {
                parameterIndex = i;
                break;
            }
        }
        if (parameterIndex == -1)
        {
            throw new ArgumentException($"No parameter of type {typeof(T).Name} given in endpoint.");
        }
        var validator = handlerContext.ApplicationServices.GetRequiredService<IValidator<T>>();
        return invocationContext =>
        {
            ValidationResult validationResult = validator.Validate(invocationContext.GetArgument<T>(parameterIndex));
            if (!validationResult.IsValid)
            {
                return ValueTask.FromResult<object?>(Results.ValidationProblem(validationResult.ToDictionary()));
            }
            return next(invocationContext);
        };
    }
}

