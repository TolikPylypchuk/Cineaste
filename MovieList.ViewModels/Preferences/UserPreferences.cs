namespace MovieList.Preferences
{
    public sealed class UserPreferences
    {
        public UIPreferences UI { get; }
        public FilePreferences File { get; }
        public LoggingPreferences Logging { get; }

        public UserPreferences(UIPreferences ui, FilePreferences file, LoggingPreferences logging)
        {
            this.UI = ui;
            this.File = file;
            this.Logging = logging;
        }
    }
}
