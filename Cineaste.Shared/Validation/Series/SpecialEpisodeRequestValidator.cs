namespace Cineaste.Shared.Validation.Series;

using static Cineaste.Basic.Constants;

public sealed class SpecialEpisodeRequestValidator : TitledRequestValidator<SpecialEpisodeRequest>
{
    public SpecialEpisodeRequestValidator()
        : base("SpecialEpisode")
    {
        this.RuleFor(req => new { req.IsWatched, req.IsReleased })
            .Must(x => x.IsWatched.Implies(x.IsReleased))
            .WithErrorCode(this.ErrorCode(req => req.IsWatched, Invalid));

        this.RuleFor(req => new { req.IsReleased, req.Year, req.Month })
            .Must(x =>
            {
                var today = DateTime.Now;
                return (x.Year, x.Month) switch
                {
                    (int year, _) when year < today.Year => x.IsReleased,
                    (int year, _) when year > today.Year => !x.IsReleased,
                    (int year, int month) when year == today.Year && month < today.Month => x.IsReleased,
                    (int year, int month) when year == today.Year && month > today.Month => !x.IsReleased,
                    _ => true
                };
            })
            .WithErrorCode(this.ErrorCode(req => req.IsReleased, Invalid));

        this.RuleFor(req => req.Channel)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Channel, Empty))
            .MaximumLength(MaxNameLength)
            .WithErrorCode(this.ErrorCode(req => req.Channel, TooLong));

        this.RuleFor(req => req.Month)
            .InclusiveBetween(1, 12)
            .WithErrorCode(this.ErrorCode(req => req.Month, Invalid));

        this.RuleFor(req => req.Year)
            .GreaterThan(MinYear)
            .WithErrorCode(this.ErrorCode(req => req.Year, TooLow));

        this.RuleFor(req => req.RottenTomatoesId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesId, Invalid));
    }
}
