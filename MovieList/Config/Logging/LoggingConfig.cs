using System.Collections.Generic;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Config.Logging
{
    public class LoggingConfig
    {
        public Dictionary<string, string> LogLevel { get; set; }
        public FileConfig File { get; set; }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.
