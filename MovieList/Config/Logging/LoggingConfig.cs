using System.Collections.Generic;

namespace MovieList.Config.Logging
{
    public class LoggingConfig
    {
        public Dictionary<string, string> LogLevel { get; set; }
        public FileConfig File { get; set; }
    }
}
