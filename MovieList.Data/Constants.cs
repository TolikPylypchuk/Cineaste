namespace MovieList.Data
{
    public static class Constants
    {
        public const int ListFileVersion = 1;

        public const int MovieMinYear = 1850;
        public const int MovieMaxYear = 2100;

        public const int SeriesMinYear = 1950;
        public const int SeriesMaxYear = 2100;

        public const int MinTitleCount = 1;
        public const int MaxTitleCount = 10;

        public const int PeriodMinNumberOfEpisodes = 1;
        public const int PeriodMaxNumberOfEpisodes = 50;

        internal const string SettingsListNameKey = "list.name";
        internal const string SettingsListVersionKey = "list.version";
        internal const string SettingsDefaultSeasonTitleKey = "season.title.default";
        internal const string SettingsDefaultSeasonOriginalTitleKey = "season.title.default-original";
    }
}
