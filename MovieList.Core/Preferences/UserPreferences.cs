using System.Collections.Generic;
using System.Globalization;

using MovieList.Data.Models;

namespace MovieList.Core.Preferences
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class UserPreferences : ISettings
    {
        public UserPreferences(FilePreferences file, DefaultsPreferences defaults, LoggingPreferences logging)
        {
            this.File = file;
            this.Defaults = defaults;
            this.Logging = logging;
        }

        public FilePreferences File { get; }
        public DefaultsPreferences Defaults { get; }
        public LoggingPreferences Logging { get; }

        string ISettings.DefaultSeasonTitle
        {
            get => this.Defaults.DefaultSeasonTitle;
            set => this.Defaults.DefaultSeasonTitle = value;
        }

        string ISettings.DefaultSeasonOriginalTitle
        {
            get => this.Defaults.DefaultSeasonOriginalTitle;
            set => this.Defaults.DefaultSeasonOriginalTitle = value;
        }

        CultureInfo ISettings.CultureInfo
        {
            get => this.Defaults.DefaultCultureInfo;
            set => this.Defaults.DefaultCultureInfo = value;
        }

        List<Kind> ISettings.Kinds =>
            this.Defaults.DefaultKinds;

        List<Tag> ISettings.Tags =>
            this.Defaults.DefaultTags;
    }
}
