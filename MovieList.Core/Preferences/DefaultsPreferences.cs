using System.Collections.Generic;

using MovieList.Data.Models;

namespace MovieList.Preferences
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class DefaultsPreferences
    {
        public DefaultsPreferences(
            string defaultSeasonTitle,
            string defaultSeasonOriginalTitle,
            List<Kind> defaultKinds,
            string defaultCultureInfo)
        {
            this.DefaultSeasonTitle = defaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
            this.DefaultKinds = defaultKinds;
            this.DefaultCultureInfo = defaultCultureInfo;
        }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginalTitle { get; set; }
        public List<Kind> DefaultKinds { get; set; }
        public string DefaultCultureInfo { get; set; }
    }
}
