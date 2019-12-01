namespace MovieList.Preferences
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class UserPreferences
    {
        public UserPreferences(FilePreferences file, DefaultsPreferences defaults, LoggingPreferences logging)
        {
            this.File = file;
            this.Defaults = defaults;
            this.Logging = logging;
        }

        public FilePreferences File { get; }
        public DefaultsPreferences Defaults { get; }
        public LoggingPreferences Logging { get; }
    }
}
