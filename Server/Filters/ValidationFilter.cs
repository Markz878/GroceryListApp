using System.Reflection;

namespace GroceryListHelper.Server.Filters;

public static class ValidatorFactory
{
    public static RouteGroupBuilder AddFluentValidation(this RouteGroupBuilder builder)
    {
        return builder.AddEndpointFilterFactory(ValidationFilter);
    }

    private static EndpointFilterDelegate ValidationFilter(EndpointFilterFactoryContext handlerContext, EndpointFilterDelegate next)
    {
        ParameterInfo[] parameters = handlerContext.MethodInfo.GetParameters();
        for (int i = 0; i < parameters.Length; i++)
        {
            Type validatorType = typeof(IValidator<>).MakeGenericType(parameters[i].ParameterType);
            IValidator? validator = handlerContext.ApplicationServices.GetService(validatorType) as IValidator;
            if (validator is not null)
            {
                ValidationDescriptor validationDescriptor = new(i, parameters[i].ParameterType, validator);
                return invocationContext =>
                {
                    object? argument = invocationContext.Arguments[validationDescriptor.ArgumentIndex];
                    if (argument is not null)
                    {
                        ValidationResult validationResult = validationDescriptor.Validator.Validate(new ValidationContext<object>(argument));
                        if (!validationResult.IsValid)
                        {
                            return ValueTask.FromResult<object?>(Results.ValidationProblem(validationResult.ToDictionary()));
                        }
                    }
                    return next(invocationContext);
                };
            }
        }
        return invocationContext => next(invocationContext);
    }

    private record ValidationDescriptor(int ArgumentIndex, Type ArgumentType, IValidator Validator);
}

