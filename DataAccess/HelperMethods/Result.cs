using System.Diagnostics;

namespace GroceryListHelper.DataAccess.HelperMethods;

public readonly struct Response<T, E> where E : Exception
{
    public bool IsSuccess { get; }
    private readonly T? _value;
    private readonly E? _error;
    public Response(T value)
    {
        _value = value;
        IsSuccess = true;
    }
    public Response(E error)
    {
        _error = error;
    }
    public static implicit operator Response<T, E>(T value)
    {
        return new(value);
    }

    public U Match<U>(Func<T, U> handleValue, Func<E, U> handleError)
    {
        if (IsSuccess)
        {
            ArgumentNullException.ThrowIfNull(_value);
            return handleValue.Invoke(_value);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(_error);
            return handleError.Invoke(_error);
        }
    }

    public Task<U> MatchAsync<U>(Func<T, Task<U>> handleValue, Func<E, Task<U>> handleError)
    {
        if (IsSuccess)
        {
            ArgumentNullException.ThrowIfNull(_value);
            return handleValue.Invoke(_value);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(_error);
            return handleError.Invoke(_error);
        }
    }
}

public readonly struct Response<T, E1, E2> where E1 : Exception where E2 : Exception
{
    public bool IsSuccess { get; }
    private readonly T? _value;
    private readonly E1? _error1;
    private readonly E2? _error2;
    public Response(T value)
    {
        _value = value;
        IsSuccess = true;
    }
    public Response(E1 error)
    {
        _error1 = error;
    }
    public Response(E2 error)
    {
        _error2 = error;
    }

    public static implicit operator Response<T, E1, E2>(T value)
    {
        return new(value);
    }

    public U Match<U>(Func<T, U> handleValue, Func<E1, U> handleError1, Func<E2, U> handleError2)
    {
        if (IsSuccess && _value is not null)
        {
            return handleValue.Invoke(_value);
        }
        else if (_error1 is not null)
        {
            return handleError1.Invoke(_error1);
        }
        else if (_error2 is not null)
        {
            return handleError2.Invoke(_error2);
        }
        throw new UnreachableException();
    }

    public Task<U> MatchAsync<U>(Func<T, Task<U>> handleValue, Func<E1, Task<U>> handleError1, Func<E2, Task<U>> handleError2)
    {
        if (IsSuccess)
        {
            ArgumentNullException.ThrowIfNull(_value);
            return handleValue.Invoke(_value);
        }
        else if (_error1 is not null)
        {
            return handleError1.Invoke(_error1);
        }
        else if (_error2 is not null)
        {
            return handleError2.Invoke(_error2);
        }
        throw new UnreachableException();
    }
}