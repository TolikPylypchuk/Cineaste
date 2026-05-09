using Cineaste.Shared.Validation.Series;

namespace Cineaste.Shared.Models.Series;

public sealed record LimitedSeriesRequest(
    ImmutableValueList<TitleRequest> Titles,
    ImmutableValueList<TitleRequest> OriginalTitles,
    ReleasePeriodRequest Period,
    SeriesWatchStatus WatchStatus,
    SeriesReleaseStatus ReleaseStatus,
    string Channel,
    Guid KindId,
    string? ImdbId,
    string? RottenTomatoesId,
    string? RottenTomatoesSubId,
    Guid? ParentFranchiseId) : IValidatable<LimitedSeriesRequest>, ITitledRequest
{
    public static IValidator<LimitedSeriesRequest> Validator { get; } = new LimitedSeriesRequestValidator();
}
