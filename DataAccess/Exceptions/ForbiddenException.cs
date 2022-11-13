namespace GroceryListHelper.DataAccess.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("User is not authorized to perform this action")
    {
    }
    public ForbiddenException(string message) : base(message)
    {
    }
    public static ForbiddenException Instance { get; } = new ForbiddenException();
}
