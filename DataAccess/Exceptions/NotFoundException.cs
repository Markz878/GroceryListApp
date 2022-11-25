namespace GroceryListHelper.DataAccess.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName) : base($"{entityName} was not found with the given key.")
    {
    }

    public static NotFoundException ForType<T>()
    {
        return new(nameof(T));
    }
}
