using Cineaste.Shared.Validation.Series;

namespace Cineaste.Shared.Models.Series;

public sealed record SeasonRequest(
    Guid? Id,
    ImmutableValueList<TitleRequest> Titles,
    ImmutableValueList<TitleRequest> OriginalTitles,
    int SequenceNumber,
    SeasonWatchStatus WatchStatus,
    SeasonReleaseStatus ReleaseStatus,
    string Channel,
    ImmutableValueList<PeriodRequest> Periods) : IValidatable<SeasonRequest>, ITitledRequest
{
    public static IValidator<SeasonRequest> Validator { get; } = new SeasonRequestValidator();
}
