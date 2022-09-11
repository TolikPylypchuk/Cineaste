namespace Cineaste.Shared.Validation.Series;

using static Cineaste.Basic.Constants;

public sealed class PeriodRequestValidator : CineasteValidator<PeriodRequest>
{
    public PeriodRequestValidator()
        : base("Period")
    {
        this.RuleFor(req => req.StartMonth)
            .InclusiveBetween(1, 12)
            .WithErrorCode(this.ErrorCode(req => req.StartMonth, Invalid));

        this.RuleFor(req => req.StartYear)
            .GreaterThan(MinYear)
            .WithErrorCode(this.ErrorCode(req => req.StartYear, TooLow));

        this.RuleFor(req => req.EndMonth)
            .InclusiveBetween(1, 12)
            .WithErrorCode(this.ErrorCode(req => req.EndMonth, Invalid));

        this.RuleFor(req => req.EndYear)
            .GreaterThan(MinYear)
            .WithErrorCode(this.ErrorCode(req => req.EndYear, TooLow));

        this.RuleFor(req => new { req.StartMonth, req.StartYear, req.EndMonth, req.EndYear })
            .Must(x => x.StartYear < x.EndYear || x.StartYear == x.EndYear && x.StartMonth <= x.EndMonth)
            .WithErrorCode(this.ErrorCode(Invalid));

        this.RuleFor(req => new { req.IsSingleDayRelease, req.StartMonth, req.StartYear, req.EndMonth, req.EndYear })
            .Must(x => x.IsSingleDayRelease.Implies(x.StartYear == x.EndYear && x.StartMonth == x.EndMonth))
            .WithErrorCode(this.ErrorCode(req => req.IsSingleDayRelease, Invalid));

        this.RuleFor(req => new { req.IsSingleDayRelease, req.EpisodeCount })
            .Must(x => (x.EpisodeCount == 1).Implies(x.IsSingleDayRelease))
            .WithErrorCode(this.ErrorCode(req => req.IsSingleDayRelease, MustBeTrue));

        this.RuleFor(req => req.EpisodeCount)
            .InclusiveBetween(1, 100)
            .WithErrorCode(this.ErrorCode(req => req.EpisodeCount, Invalid));

        this.RuleFor(req => req.RottenTomatoesId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesId, Invalid));
    }
}
