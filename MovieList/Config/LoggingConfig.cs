using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace MovieList.Config
{
    public class LoggingConfig
    {
        public Dictionary<string, LogLevel> LogLevel { get; set; }
        public FileConfig File { get; set; }
    }
}
