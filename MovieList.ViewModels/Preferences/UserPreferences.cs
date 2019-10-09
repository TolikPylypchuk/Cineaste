namespace MovieList.Preferences
{
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
