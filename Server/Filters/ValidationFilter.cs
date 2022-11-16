namespace GroceryListHelper.Server.Filters;

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
        IValidator<T> validator = handlerContext.ApplicationServices.GetRequiredService<IValidator<T>>();
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

