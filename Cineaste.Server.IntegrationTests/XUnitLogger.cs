namespace Cineaste.Server;

using System.Text;

using Microsoft.Extensions.Logging;

public class XUnitLogger : ILogger
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly LoggerExternalScopeProvider scopeProvider;
    private readonly string categoryName;

    public static ILogger CreateLogger(Type type, ITestOutputHelper testOutputHelper) =>
        new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), type.FullName ?? String.Empty);

    public static ILogger CreateLogger(ITestOutputHelper testOutputHelper) =>
        new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), String.Empty);

    public static ILogger<T> CreateLogger<T>(ITestOutputHelper testOutputHelper) =>
        new XUnitLogger<T>(testOutputHelper, new LoggerExternalScopeProvider());

    public XUnitLogger(
        ITestOutputHelper testOutputHelper,
        LoggerExternalScopeProvider scopeProvider,
        string categoryName)
    {
        this.testOutputHelper = testOutputHelper;
        this.scopeProvider = scopeProvider;
        this.categoryName = categoryName;
    }

    public bool IsEnabled(LogLevel logLevel) =>
        logLevel != LogLevel.None;

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull =>
        this.scopeProvider.Push(state);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var sb = new StringBuilder();

        sb.Append(this.GetLogLevelString(logLevel))
            .Append(" [")
            .Append(this.categoryName)
            .Append("] ")
            .Append(formatter(state, exception));

        if (exception != null)
        {
            sb.Append('\n').Append(exception);
        }

        this.scopeProvider.ForEachScope(
            (scope, state) =>
            {
                state.Append("\n => ");
                state.Append(scope);
            },
            sb);

        this.testOutputHelper.WriteLine(sb.ToString());
    }

    private string GetLogLevelString(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
}

public sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
{
    public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider)
        : base(testOutputHelper, scopeProvider, typeof(T).FullName ?? String.Empty)
    {
    }
}
