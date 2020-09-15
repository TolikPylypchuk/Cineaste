using System.Collections.Generic;
using System.Globalization;

using MovieList.Core.Preferences;
using MovieList.Data;
using MovieList.Data.Models;

namespace MovieList.Core.Models
{
    public sealed record CreateFileModel(string File, string ListName);

    public sealed record OpenFileModel(string File)
    {
        public bool IsExternal { get; init; }
    }

    public sealed record SettingsModel(Settings Settings, List<Kind> Kinds) : ISettings
    {
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
