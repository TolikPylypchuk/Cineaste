namespace Cineaste.Converters;

using Serilog.Events;

public sealed class LogLevelConverter : EnumConverter<LogEventLevel>
{
    protected override Dictionary<LogEventLevel, string> CreateConverterDictionary() =>
        new()
        {
            [LogEventLevel.Verbose] = Messages.LogLevelVerbose,
            [LogEventLevel.Debug] = Messages.LogLevelDebug,
            [LogEventLevel.Information] = Messages.LogLevelInformation,
            [LogEventLevel.Warning] = Messages.LogLevelWarning,
            [LogEventLevel.Error] = Messages.LogLevelError,
            [LogEventLevel.Fatal] = Messages.LogLevelFatal
        };
}
