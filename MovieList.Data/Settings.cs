using System.Globalization;

using static MovieList.Data.Constants;

namespace MovieList.Data
{
    public sealed class Settings
    {
        public Settings(
            string listName,
            int listVersion,
            string defaultSeasonTitle,
            string defaultSeasonOriginalTitle,
            string cultureInfo)
        {
            this.ListName = listName;
            this.ListVersion = listVersion;
            this.DefaultSeasonTitle = defaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
            this.CultureInfo = new CultureInfo(cultureInfo);
        }

        public string ListName { get; set; }
        public int ListVersion { get; }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginalTitle { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public string GetSeasonTitle(int num)
            => this.DefaultSeasonTitle.Replace(SeasonNumberPlaceholder, num.ToString());

        public string GetSeasonOriginalTitle(int num)
            => this.DefaultSeasonOriginalTitle.Replace(SeasonNumberPlaceholder, num.ToString());
    }
}
