namespace Cineaste.Shared.Models.Series;

public sealed record SeasonModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    int SequenceNumber,
    SeasonWatchStatus WatchStatus,
    SeasonReleaseStatus ReleaseStatus,
    string Channel,
    ImmutableList<PeriodModel> Periods) : ISeriesComponentModel;
