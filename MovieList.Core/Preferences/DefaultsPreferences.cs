using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
            List<Tag> defaultTags,
            CultureInfo defaultCultureInfo)
        {
            this.DefaultSeasonTitle = defaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
            this.DefaultKinds = defaultKinds;
            this.DefaultTags = defaultTags;
            this.DefaultCultureInfo = defaultCultureInfo;
            this.AdjustTags();
        }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginalTitle { get; set; }
        public List<Kind> DefaultKinds { get; set; }
        public List<Tag> DefaultTags { get; set; }
        public CultureInfo DefaultCultureInfo { get; set; }

        private void AdjustTags()
        {
            var tagsByNameCategory = this.DefaultTags
                .ToDictionary(tag => (tag.Name, tag.Category), tag => tag);

            foreach (var tag in this.DefaultTags)
            {
                tag.ImpliedTags = tag.ImpliedTags
                    .Select(tag => tagsByNameCategory[(tag.Name, tag.Category)])
                    .ToHashSet();
            }
        }
    }
}
