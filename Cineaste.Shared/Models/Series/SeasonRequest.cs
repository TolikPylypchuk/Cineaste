namespace Cineaste.Shared.Models.Series;

using Cineaste.Shared.Validation.Series;

public sealed record SeasonRequest(
    Guid? Id,
    ImmutableList<TitleRequest> Titles,
    ImmutableList<TitleRequest> OriginalTitles,
    int SequenceNumber,
    SeasonWatchStatus WatchStatus,
    SeasonReleaseStatus ReleaseStatus,
    string Channel,
    ImmutableList<PeriodRequest> Periods) : IValidatable<SeasonRequest>, ITitledRequest
{
    public static IValidator<SeasonRequest> CreateValidator() =>
        new SeasonRequestValidator();
}
