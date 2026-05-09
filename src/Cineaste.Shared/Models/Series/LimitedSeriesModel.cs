namespace Cineaste.Shared.Models.Series;

public sealed record LimitedSeriesModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    ReleasePeriodModel Period,
    SeriesWatchStatus WatchStatus,
    SeriesReleaseStatus ReleaseStatus,
    string Channel,
    ListKindModel Kind,
    string? ImdbId,
    string? RottenTomatoesId,
    string? RottenTomatoesSubId,
    string ListItemColor,
    int ListSequenceNumber,
    string? PosterUrl,
    FranchiseItemInfoModel? FranchiseItem) : IIdentifyableModel, ITitledModel;
