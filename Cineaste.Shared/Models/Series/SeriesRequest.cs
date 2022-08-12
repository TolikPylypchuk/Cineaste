namespace Cineaste.Shared.Models.Series;

public sealed record SeriesRequest(
    Guid ListId,
    ImmutableList<TitleRequest> Titles,
    ImmutableList<TitleRequest> OriginalTitles,
    SeriesWatchStatus WatchStatus,
    SeriesReleaseStatus ReleaseStatus,
    ListKindModel Kind,
    bool IsMiniseries,
    ImmutableList<SeasonRequest> Seasons,
    ImmutableList<SpecialEpisodeRequest> SpecialEpisodes,
    string? ImdbId,
    string? RottenTomatoesId);
