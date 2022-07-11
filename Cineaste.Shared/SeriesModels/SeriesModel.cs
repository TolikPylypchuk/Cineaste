namespace Cineaste.Shared.SeriesModels;

public sealed record SeriesModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    SeriesWatchStatus WatchStatus,
    SeriesReleaseStatus ReleaseStatus,
    ListKindModel Kind,
    bool IsMiniseries,
    string? ImdbId,
    string? RottenTomatoesLink,
    string DisplayNumber);
