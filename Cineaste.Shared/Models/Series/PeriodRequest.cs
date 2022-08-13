namespace Cineaste.Shared.Models.Series;

using Cineaste.Shared.Validation.Series;

public sealed record PeriodRequest(
    Guid? Id,
    int StartMonth,
    int StartYear,
    int EndMonth,
    int EndYear,
    int EpisodeCount,
    bool IsSingleDayRelease,
    string? RottenTomatoesId) : IValidatable<PeriodRequest>
{
    public static IValidator<PeriodRequest> CreateValidator() =>
        new PeriodRequestValidator();
}
