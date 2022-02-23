namespace GroceryListHelper.Shared.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("User is not authorized to perform this action")
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public static void ThrowIfNotAuthorized(bool authorizedCondition)
    {
        if (!authorizedCondition)
        {
            throw new ForbiddenException();
        }
    }
}
