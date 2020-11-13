using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            CultureInfo cultureInfo,
            ListSortOrder defaultFirstSortOrder,
            ListSortOrder defaultSecondSortOrder,
            ListSortDirection defaultFirstSortDirection,
            ListSortDirection defaultSecondSortDirection)
        {
            this.ListName = listName;
            this.ListVersion = listVersion;

            this.DefaultSeasonTitle = defaultSeasonTitle;
            this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;

            this.CultureInfo = cultureInfo;

            this.DefaultFirstSortOrder = defaultFirstSortOrder;
            this.DefaultSecondSortOrder = defaultSecondSortOrder;

            this.DefaultFirstSortDirection = defaultFirstSortDirection;
            this.DefaultSecondSortDirection = defaultSecondSortDirection;
        }

        internal static Settings FromDictionary(IDictionary<string, string> settings) =>
            new Settings(
                settings[SettingsListNameKey],
                Int32.Parse(settings[SettingsListVersionKey]),
                settings[SettingsDefaultSeasonTitleKey],
                settings[SettingsDefaultSeasonOriginalTitleKey],
                new CultureInfo(settings[SettingsListCultureKey]),
                Enum.Parse<ListSortOrder>(settings[SettingsDefaultFirstSortOrderKey]),
                Enum.Parse<ListSortOrder>(settings[SettingsDefaultSecondSortOrderKey]),
                Enum.Parse<ListSortDirection>(settings[SettingsDefaultFirstSortDirectionKey]),
                Enum.Parse<ListSortDirection>(settings[SettingsDefaultSecondSortDirectionKey]));

        public string ListName { get; set; }
        public int ListVersion { get; }

        public string DefaultSeasonTitle { get; set; }
        public string DefaultSeasonOriginalTitle { get; set; }

        public ListSortOrder DefaultFirstSortOrder { get; set; }
        public ListSortOrder DefaultSecondSortOrder { get; set; }

        public ListSortDirection DefaultFirstSortDirection { get; set; }
        public ListSortDirection DefaultSecondSortDirection { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public string GetSeasonTitle(int num) =>
            this.DefaultSeasonTitle.Replace(SeasonNumberPlaceholder, num.ToString());

        public string GetSeasonOriginalTitle(int num) =>
            this.DefaultSeasonOriginalTitle.Replace(SeasonNumberPlaceholder, num.ToString());
    }
}
