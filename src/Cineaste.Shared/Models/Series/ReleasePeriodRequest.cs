using Cineaste.Shared.Validation.Series;

namespace Cineaste.Shared.Models.Series;

public sealed record ReleasePeriodRequest(
    int StartMonth,
    int StartYear,
    int EndMonth,
    int EndYear,
    int EpisodeCount,
    bool IsSingleDayRelease) : IValidatable<ReleasePeriodRequest>
{
    public static IValidator<ReleasePeriodRequest> Validator { get; } = new ReleasePeriodRequestValidator();
}
