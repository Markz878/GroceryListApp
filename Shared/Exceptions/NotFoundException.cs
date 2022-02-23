using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GroceryListHelper.Shared.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName) : base($"{entityName} was not found")
    {
    }

    public NotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    public static void ThrowIfNull<T>([NotNull] T entity, [CallerArgumentExpression("entity")] string entityName = "")
    {
        if (entity == null)
        {
            throw new NotFoundException(string.Concat(entityName[0].ToString().ToUpper(), entityName.AsSpan(1)));
        }
    }
}
