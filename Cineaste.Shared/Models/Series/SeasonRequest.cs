namespace Cineaste.Shared.Models.Series;

public sealed record SeasonRequest(
    Guid? Id,
    ImmutableList<TitleRequest> Titles,
    ImmutableList<TitleRequest> OriginalTitles,
    int SequenceNumber,
    SeasonWatchStatus WatchStatus,
    SeasonReleaseStatus ReleaseStatus,
    string Channel,
    ImmutableList<PeriodRequest> Periods);
