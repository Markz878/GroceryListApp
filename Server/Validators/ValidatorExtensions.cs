namespace GroceryListHelper.Server.Validators;

public static class ValidatorExtensions
{
    public static bool FindsError<T>(this IValidator<T> validator, T input, out string errorMessage)
    {
        ValidationResult validationResult = validator.Validate(input);
        errorMessage = string.Empty;
        if (!validationResult.IsValid)
        {
            errorMessage = "Input validation failed. " + string.Join(' ', validationResult.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage}"));
            return true;
        }
        return false;
    }
}
