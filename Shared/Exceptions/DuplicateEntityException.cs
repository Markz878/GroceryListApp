namespace GroceryListHelper.Shared.Exceptions;

/// <summary>
/// An exception that is thrown when trying to create or update an entity which creates an unallowed duplicate entity
/// </summary>
public class DuplicateEntityException : Exception
{
    public DuplicateEntityException() : base("Cannot create a duplicate value")
    {
    }

    public DuplicateEntityException(string message) : base(message)
    {
    }

    public DuplicateEntityException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
