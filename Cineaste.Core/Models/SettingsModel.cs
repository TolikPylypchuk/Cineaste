namespace Cineaste.Core.Models;

public sealed record SettingsModel(Settings Settings, List<Kind> Kinds, List<Tag> Tags) : ISettings
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

    ListSortOrder ISettings.DefaultFirstSortOrder
    {
        get => this.Settings.DefaultFirstSortOrder;
        set => this.Settings.DefaultFirstSortOrder = value;
    }

    ListSortOrder ISettings.DefaultSecondSortOrder
    {
        get => this.Settings.DefaultSecondSortOrder;
        set => this.Settings.DefaultSecondSortOrder = value;
    }

    ListSortDirection ISettings.DefaultFirstSortDirection
    {
        get => this.Settings.DefaultFirstSortDirection;
        set => this.Settings.DefaultFirstSortDirection = value;
    }

    ListSortDirection ISettings.DefaultSecondSortDirection
    {
        get => this.Settings.DefaultSecondSortDirection;
        set => this.Settings.DefaultSecondSortDirection = value;
    }
}
