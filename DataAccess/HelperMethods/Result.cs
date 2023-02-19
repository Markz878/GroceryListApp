//namespace GroceryListHelper.DataAccess.HelperMethods;

//public readonly struct Result<T>
//{
//    public bool IsSuccess { get; }
//    private readonly T? Value;
//    private readonly Exception? exception;

//    /// <summary>
//    /// Constructor of a concrete value
//    /// </summary>
//    public Result(T value)
//    {
//        IsSuccess = true;
//        Value = value;
//        exception = null;
//    }

//    /// <summary>
//    /// Constructor of an error value
//    /// </summary>
//    public Result(Exception e)
//    {
//        IsSuccess = false;
//        exception = e;
//        Value = default;
//    }

//    public static implicit operator Result<T>(T value)
//    {
//        return new(value);
//    }

//    public override string ToString()
//    {
//        return IsSuccess ? Value?.ToString() ?? "(null)" : exception?.ToString() ?? "(null exception)";
//    }

//    public U Match<U>(Func<T, U> Succ, Func<Exception, U> Fail)
//    {
//        if (IsSuccess)
//        {
//            ArgumentNullException.ThrowIfNull(Value);
//            return Succ(Value);
//        }
//        else
//        {
//            ArgumentNullException.ThrowIfNull(exception);
//            return Fail(exception);
//        }
//    }
//}
