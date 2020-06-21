using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Properties;

using ReactiveUI;

using Serilog.Events;

namespace MovieList.Converters
{
    public sealed class LogLevelConverter : IBindingTypeConverter
    {
        private readonly Dictionary<LogEventLevel, string> levelToString;
        private readonly Dictionary<string, LogEventLevel> stringToLevel;

        public LogLevelConverter()
        {
            this.levelToString = new Dictionary<LogEventLevel, string>
            {
                [LogEventLevel.Verbose] = Messages.LogLevelVerbose,
                [LogEventLevel.Debug] = Messages.LogLevelDebug,
                [LogEventLevel.Information] = Messages.LogLevelInformation,
                [LogEventLevel.Warning] = Messages.LogLevelWarning,
                [LogEventLevel.Error] = Messages.LogLevelError,
                [LogEventLevel.Fatal] = Messages.LogLevelFatal
            };

            this.stringToLevel = levelToString.ToDictionary(e => e.Value, e => e.Key);
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
            => fromType == typeof(LogEventLevel) || toType == typeof(LogEventLevel) ||
                fromType == typeof(int) || toType == typeof(int)
                ? 1000
                : 0;

        public bool TryConvert(object from, Type toType, object conversionHint, out object? result)
        {
            switch (from)
            {
                case LogEventLevel level:
                    result = this.levelToString[level];
                    return true;
                case int level:
                    result = this.levelToString[(LogEventLevel)level];
                    return true;
                case string str when toType == typeof(LogEventLevel):
                    result = this.stringToLevel[str];
                    return true;
                case string str when toType == typeof(int):
                    result = (int)this.stringToLevel[str];
                    return true;
                default:
                    result = null;
                    return false;
            }
        }
    }
}
