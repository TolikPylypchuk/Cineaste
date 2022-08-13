namespace Cineaste.Shared.Validation.Movie;

using static Cineaste.Basic.Constants;

public sealed class MovieRequestValidator : TitledRequestValidator<MovieRequest>
{
    public MovieRequestValidator()
        : base("Movie")
    {
        this.RuleFor(req => new { req.IsWatched, req.IsReleased })
            .Must(x => x.IsWatched.Implies(x.IsReleased))
            .WithErrorCode(this.ErrorCode(req => req.IsWatched, Invalid));

        this.RuleFor(req => new { req.IsReleased, req.Year })
            .Must(x => x.Year switch
            {
                int year when year < DateTime.Now.Year => x.IsReleased,
                int year when year > DateTime.Now.Year => !x.IsReleased,
                _ => true
            })
            .WithErrorCode(this.ErrorCode(req => req.IsReleased, Invalid));

        this.RuleFor(req => req.Year)
            .GreaterThan(MinYear)
            .WithErrorCode(this.ErrorCode(req => req.Year, TooLow));

        this.RuleFor(req => req.ImdbId)
            .Matches(ImdbIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.ImdbId, Invalid));

        this.RuleFor(req => req.RottenTomatoesId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesId, Invalid));
    }
}
