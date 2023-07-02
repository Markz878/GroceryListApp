namespace GroceryListHelper.DataAccess.Exceptions;

public readonly struct NotFoundError
{
    private readonly string? valueNotFound;
    public string? Message => valueNotFound is null ? null : $"{valueNotFound} was not found.";
    public NotFoundError()
    {

    }
    public NotFoundError(string valueNotFound)
    {
        this.valueNotFound = valueNotFound;
    }
}
