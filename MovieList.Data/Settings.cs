namespace MovieList.Data
{
    public sealed class Settings
    {
        public Settings(
            string listName,
            int listVersion,
            string colorForNotWatched,
            string colorForNotReleased,
            string defaultSeasonTitle,
            string defaultSeasonOriginalTitle)
        {
            this.ListName = listName;
            this.ListVersion = listVersion;
            this.ColorForNotWatched = colorForNotWatched;
            this.ColorForNotReleased = colorForNotReleased;
            this.DefaultSeasonTitle = defaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
        }

        public string ListName { get; set; }
        public int ListVersion { get; }

        public string ColorForNotWatched { get; set; }
        public string ColorForNotReleased { get; set; }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginalTitle { get; set; }
    }
}
