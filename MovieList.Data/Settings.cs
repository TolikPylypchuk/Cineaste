using static MovieList.Data.Constants;

namespace MovieList.Data
{
    public sealed class Settings
    {
        public Settings(
            string listName,
            int listVersion,
            string defaultSeasonTitle,
            string defaultSeasonOriginalTitle)
        {
            this.ListName = listName;
            this.ListVersion = listVersion;
            this.DefaultSeasonTitle = defaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
        }

        public string ListName { get; set; }
        public int ListVersion { get; }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginalTitle { get; set; }

        public string GetSeasonTitle(int num)
            => this.DefaultSeasonTitle.Replace(SeasonNumberPlaceholder, num.ToString());

        public string GetSeasonOriginalTitle(int num)
            => this.DefaultSeasonOriginalTitle.Replace(SeasonNumberPlaceholder, num.ToString());
    }
}
