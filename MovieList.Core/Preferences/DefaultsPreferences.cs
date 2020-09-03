using System.Collections.Generic;
using System.Globalization;

using MovieList.Data.Models;

namespace MovieList.Core.Preferences
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class DefaultsPreferences
    {
        public DefaultsPreferences(
            string defaultSeasonTitle,
            string defaultSeasonOriginalTitle,
            List<Kind> defaultKinds,
            CultureInfo defaultCultureInfo)
        {
            this.DefaultSeasonTitle = defaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
            this.DefaultKinds = defaultKinds;
            this.DefaultCultureInfo = defaultCultureInfo;
        }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginalTitle { get; set; }
        public List<Kind> DefaultKinds { get; set; }
        public CultureInfo DefaultCultureInfo { get; set; }
    }
}
