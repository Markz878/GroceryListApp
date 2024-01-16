namespace GroceryListHelper.Tests.IntegrationTests.Infrastucture;

internal class XUnitLoggingProvider(ITestOutputHelper testOutputHelper) : ILoggerProvider
{
    private readonly ITestOutputHelper testOutputHelper = testOutputHelper;
    private readonly LoggerExternalScopeProvider scopeProvider = new();

    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(testOutputHelper, scopeProvider, categoryName);
    }

    public void Dispose()
    {
    }
}

internal class XUnitLogger<T>(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider) : XUnitLogger(testOutputHelper, scopeProvider, typeof(T).FullName ?? throw new ArgumentException("ILogger generic type T name is null")), ILogger<T>
{
}

internal class XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider, string categoryName) : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    private readonly string _categoryName = categoryName;
    private readonly LoggerExternalScopeProvider _scopeProvider = scopeProvider;

    public static ILogger CreateLogger(ITestOutputHelper testOutputHelper)
    {
        return new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), "");
    }

    public static ILogger<T> CreateLogger<T>(ITestOutputHelper testOutputHelper)
    {
        return new XUnitLogger<T>(testOutputHelper, new LoggerExternalScopeProvider());
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

            _testOutputHelper.WriteLine(sb.ToString());
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
