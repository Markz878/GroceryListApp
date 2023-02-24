namespace GroceryListHelper.DataAccess.Exceptions;

public readonly struct NotFound
{
    private readonly string? entity;
    public string Message => entity is null ? throw new ArgumentNullException() : $"{entity} was not found.";
    public NotFound(string entityName)
    {
        entity = entityName;
    }
    public NotFound()
    {
    }
}
