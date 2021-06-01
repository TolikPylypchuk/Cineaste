namespace Cineaste.Core.Preferences
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class LoggingPreferences
    {
        public LoggingPreferences(string logPath, int minLogLevel)
        {
            this.LogPath = logPath;
            this.MinLogLevel = minLogLevel;
        }

        public string LogPath { get; set; }
        public int MinLogLevel { get; set; }
    }
}
