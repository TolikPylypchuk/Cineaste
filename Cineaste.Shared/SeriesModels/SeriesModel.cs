namespace Cineaste.Shared.SeriesModels;

using System.Text.Json.Serialization;

public sealed record SeriesModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    SeriesWatchStatus WatchStatus,
    SeriesReleaseStatus ReleaseStatus,
    ListKindModel Kind,
    bool IsMiniseries,
    ImmutableList<SeasonModel> Seasons,
    ImmutableList<SpecialEpisodeModel> SpecialEpisodes,
    string? ImdbId,
    string? RottenTomatoesLink,
    string DisplayNumber)
{
    [JsonIgnore]
    public ImmutableList<SeriesComponentModel> Components =>
        this.Seasons.Cast<SeriesComponentModel>()
            .Concat(this.SpecialEpisodes.Cast<SeriesComponentModel>())
            .OrderBy(component => component.SequenceNumber)
            .ToImmutableList();

    [JsonIgnore]
    public int StartYear =>
        this.Seasons
            .Select(season => season.Periods.Min(period => period.StartYear))
            .Concat(this.SpecialEpisodes.Select(episode => episode.Year))
            .Min();

    [JsonIgnore]
    public int EndYear =>
        this.Seasons
            .Select(season => season.Periods.Max(period => period.EndYear))
            .Concat(this.SpecialEpisodes.Select(episode => episode.Year))
            .Max();
}
