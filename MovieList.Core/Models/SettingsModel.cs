using System.Collections.Generic;
using System.Globalization;

using MovieList.Data;
using MovieList.Data.Models;
using MovieList.Preferences;

namespace MovieList.Models
{
    public class SettingsModel : ISettings
    {
        public SettingsModel(Settings settings, List<Kind> kinds)
        {
            this.Settings = settings;
            this.Kinds = kinds;
        }

        public Settings Settings { get; }
        public List<Kind> Kinds { get; }

        string ISettings.DefaultSeasonTitle
        {
            get => this.Settings.DefaultSeasonTitle;
            set => this.Settings.DefaultSeasonTitle = value;
        }

        string ISettings.DefaultSeasonOriginalTitle
        {
            get => this.Settings.DefaultSeasonOriginalTitle;
            set => this.Settings.DefaultSeasonOriginalTitle = value;
        }

        CultureInfo ISettings.CultureInfo
        {
            get => this.Settings.CultureInfo;
            set => this.Settings.CultureInfo = value;
        }

        List<Kind> ISettings.Kinds
            => this.Kinds;
    }
}
