using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Cineaste.Data;
using Cineaste.Data.Models;

namespace Cineaste.Core.Preferences
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

        List<Kind> ISettings.Kinds =>
            this.Defaults.DefaultKinds;

        List<Tag> ISettings.Tags =>
            this.Defaults.DefaultTags;

        CultureInfo ISettings.CultureInfo
        {
            get => this.Defaults.DefaultCultureInfo;
            set => this.Defaults.DefaultCultureInfo = value;
        }

        ListSortOrder ISettings.DefaultFirstSortOrder
        {
            get => this.Defaults.DefaultFirstSortOrder;
            set => this.Defaults.DefaultFirstSortOrder = value;
        }

        ListSortOrder ISettings.DefaultSecondSortOrder
        {
            get => this.Defaults.DefaultSecondSortOrder;
            set => this.Defaults.DefaultSecondSortOrder = value;
        }

        ListSortDirection ISettings.DefaultFirstSortDirection
        {
            get => this.Defaults.DefaultFirstSortDirection;
            set => this.Defaults.DefaultFirstSortDirection = value;
        }

        ListSortDirection ISettings.DefaultSecondSortDirection
        {
            get => this.Defaults.DefaultSecondSortDirection;
            set => this.Defaults.DefaultSecondSortDirection = value;
        }
    }
}
