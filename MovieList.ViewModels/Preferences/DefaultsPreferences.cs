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
            List<Kind> defaultKinds)
        {
            this.DefaultSeasonTitle = defaultSeasonTitle;
            this.DefaultSeasonOriginaltitle = defaultSeasonOriginalTitle;
            this.DefaultKinds = defaultKinds;
        }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginaltitle { get; set; }
        public List<Kind> DefaultKinds { get; set; }
    }
}
