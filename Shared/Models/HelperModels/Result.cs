﻿using System.Diagnostics;

namespace GroceryListHelper.Shared.Models.HelperModels;

public readonly struct Result<T, E>
{
    public bool IsSuccess { get; }
    private readonly T? _value;
    private readonly E? _error;
    public Result(T value)
    {
        _value = value;
        IsSuccess = true;
    }

    public Result(E error) => _error = error;

    public static implicit operator Result<T, E>(T value) => new(value);

    public static implicit operator Result<T, E>(E value) => new(value);

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

    public void Match(Action<T> handleValue, Action<E> handleError)
    {
        if (IsSuccess)
        {
            ArgumentNullException.ThrowIfNull(_value);
            handleValue.Invoke(_value);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(_error);
            handleError.Invoke(_error);
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

public readonly struct Result<T, E1, E2>
{
    public bool IsSuccess { get; }
    private readonly T? _value;
    private readonly E1? _error1;
    private readonly E2? _error2;
    private readonly bool isError1;
    private readonly bool isError2;

    public Result(T value)
    {
        _value = value;
        IsSuccess = true;
    }
    public Result(E1 error) 
    { 
        _error1 = error; 
        isError1 = true; 
    }
    public Result(E2 error)
    {
        _error2 = error;
        isError2 = true;
    }
    public static implicit operator Result<T, E1, E2>(T value) => new(value);

    public static implicit operator Result<T, E1, E2>(E1 value) => new(value);

    public static implicit operator Result<T, E1, E2>(E2 value) => new(value);

    public U Match<U>(Func<T, U> handleValue, Func<E1, U> handleError1, Func<E2, U> handleError2)
    {
        if (IsSuccess && _value is not null)
        {
            return handleValue.Invoke(_value);
        }
        else if (isError1 && _error1 is not null)
        {
            return handleError1.Invoke(_error1);
        }
        else if (isError2 && _error2 is not null)
        {
            return handleError2.Invoke(_error2);
        }
        throw new UnreachableException();
    }

    public Task<U> MatchAsync<U>(Func<T, Task<U>> handleValue, Func<E1, Task<U>> handleError1, Func<E2, Task<U>> handleError2)
    {
        if (IsSuccess && _value is not null)
        {
            return handleValue.Invoke(_value);
        }
        else if (isError1 && _error1 is not null)
        {
            return handleError1.Invoke(_error1);
        }
        else if (isError2 && _error2 is not null)
        {
            return handleError2.Invoke(_error2);
        }
        throw new UnreachableException();
    }
}