﻿using Microsoft.Extensions.Logging;
using System.Text;
namespace GroceryListHelper.E2ETests.Infrastructure;

internal class XUnitLoggingProvider : ILoggerProvider
{
    private readonly Func<ITestOutputHelper> testOutputHelperGetter;
    private readonly LoggerExternalScopeProvider scopeProvider = new();

    public XUnitLoggingProvider(Func<ITestOutputHelper> testOutputHelperGetter)
    {
        this.testOutputHelperGetter = testOutputHelperGetter;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(testOutputHelperGetter, scopeProvider, categoryName);
    }

    public void Dispose()
    {
    }
}

internal class XUnitLogger : ILogger
{
    private readonly Func<ITestOutputHelper> _testOutputHelperGetter;
    private readonly string _categoryName;
    private readonly LoggerExternalScopeProvider _scopeProvider;

    public static ILogger CreateLogger(Func<ITestOutputHelper> testOutputHelperGetter)
    {
        return new XUnitLogger(testOutputHelperGetter, new LoggerExternalScopeProvider(), "");
    }

    public XUnitLogger(Func<ITestOutputHelper> testOutputHelperGetter, LoggerExternalScopeProvider scopeProvider, string categoryName)
    {
        _testOutputHelperGetter = testOutputHelperGetter;
        _scopeProvider = scopeProvider;
        _categoryName = categoryName;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _scopeProvider.Push(state);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        try
        {
            StringBuilder sb = new();
            sb.Append(GetLogLevelString(logLevel))
              .Append(" [").Append(_categoryName).Append("] ")
              .Append(formatter(state, exception));

            if (exception != null)
            {
                sb.Append('\n').Append(exception);
            }

            // Append scopes
            _scopeProvider.ForEachScope((scope, state) =>
            {
                state.Append("\n => ");
                state.Append(scope);
            }, sb);

            _testOutputHelperGetter().WriteLine(sb.ToString());
        }
        catch
        {
        }
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }
}
