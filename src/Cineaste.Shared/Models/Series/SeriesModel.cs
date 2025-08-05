using System.Text.Json.Serialization;

namespace Cineaste.Shared.Models.Series;

public sealed record SeriesModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    SeriesWatchStatus WatchStatus,
    SeriesReleaseStatus ReleaseStatus,
    ListKindModel Kind,
    ImmutableList<SeasonModel> Seasons,
    ImmutableList<SpecialEpisodeModel> SpecialEpisodes,
    string? ImdbId,
    string? RottenTomatoesId,
    string ListItemColor,
    int ListSequenceNumber,
    string? PosterUrl,
    FranchiseItemInfoModel? FranchiseItem) : IIdentifyableModel, ITitledModel
{
    [JsonIgnore]
    public ImmutableList<ISeriesComponentModel> Components =>
        [.. this.Seasons.Cast<ISeriesComponentModel>()
            .Concat(this.SpecialEpisodes.Cast<ISeriesComponentModel>())
            .OrderBy(component => component.SequenceNumber)];

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
