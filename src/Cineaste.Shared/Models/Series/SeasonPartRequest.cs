using Cineaste.Shared.Validation.Series;

namespace Cineaste.Shared.Models.Series;

public sealed record SeasonPartRequest(
    Guid? Id,
    ReleasePeriodRequest Period,
    string? RottenTomatoesId) : IValidatable<SeasonPartRequest>
{
    public static IValidator<SeasonPartRequest> Validator { get; } = new SeasonPartRequestValidator();
}
