using static Cineaste.Common.Constants;

namespace Cineaste.Shared.Validation.Series;

public sealed class SeasonPartRequestValidator : CineasteValidator<SeasonPartRequest>
{
    public SeasonPartRequestValidator()
        : base("SeasonPart")
    {
        this.RuleFor(req => req.Period)
            .SetValidator(ReleasePeriodRequest.Validator);

        this.RuleFor(req => req.RottenTomatoesId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesId, Invalid));
    }
}
