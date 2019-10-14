namespace MovieList.Preferences
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class UserPreferences
    {
        public FilePreferences File { get; }
        public LoggingPreferences Logging { get; }

        public UserPreferences(FilePreferences file, LoggingPreferences logging)
        {
            this.File = file;
            this.Logging = logging;
        }
    }
}
