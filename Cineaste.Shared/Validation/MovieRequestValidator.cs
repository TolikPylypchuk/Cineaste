namespace Cineaste.Shared.Validation;

using System.Linq.Expressions;

using static Cineaste.Basic.Constants;

public class MovieRequestValidator : CineasteValidator<MovieRequest>
{
    private const string Names = nameof(Names);
    private const string Priorities = nameof(Priorities);

    private static readonly IValidator<TitleRequest> titleValidator = TitleRequest.CreateValidator();

    public MovieRequestValidator()
        : base("Movie")
    {
        this.AddRulesForTitles(req => req.Titles);
        this.AddRulesForTitles(req => req.OriginalTitles);

        this.RuleFor(req => new { req.IsWatched, req.IsReleased })
            .Must(x => BeReleasedIfWatched(x.IsWatched, x.IsReleased))
            .WithErrorCode(this.ErrorCode(req => req.IsWatched, Invalid));

        this.RuleFor(req => req.Year)
            .GreaterThan(1900)
            .WithErrorCode(this.ErrorCode(req => req.Year, TooLow));

        this.RuleFor(req => req.ImdbId)
            .Matches(ImdbIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.ImdbId, Invalid));

        this.RuleFor(req => req.RottenTomatoesId)
            .Matches(RottenTomatoesIdRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesId, Invalid));
    }

    private static bool HaveDifferentNames(IEnumerable<TitleRequest> titles) =>
        titles.DistinctBy(title => title.Name).Count() == titles.Count();

    private static bool HaveDifferentPriorities(IEnumerable<TitleRequest> titles) =>
        titles.DistinctBy(title => title.Priority).Count() == titles.Count();

    private static bool BeReleasedIfWatched(bool isWatched, bool isReleased) =>
        !isWatched || isReleased;

    private void AddRulesForTitles(Expression<Func<MovieRequest, IEnumerable<TitleRequest>>> titles)
    {
        this.RuleFor(titles)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(titles, Empty))
            .Must(HaveDifferentNames)
            .WithErrorCode(this.ErrorCode(titles, Distinct, Names))
            .Must(HaveDifferentPriorities)
            .WithErrorCode(this.ErrorCode(titles, Distinct, Priorities));

        this.RuleForEach(titles)
            .SetValidator(titleValidator);
    }
}
