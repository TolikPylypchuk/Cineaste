namespace Cineaste.Shared.Validation;

using System.Linq.Expressions;

using static Cineaste.Basic.Constants;

public class MovieRequestValidator : CineasteValidator<MovieRequest>
{
    private const string NotOriginal = nameof(NotOriginal);
    private const string Original = nameof(Original);
    private const string Names = nameof(Original);
    private const string Priorities = nameof(Original);

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

        this.RuleFor(req => req.RottenTomatoesLink)
            .Matches(RottenTomatoesLinkRegex)
            .WithErrorCode(this.ErrorCode(req => req.RottenTomatoesLink, Invalid));
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
            .WithErrorCode(this.ErrorCode(titles, Original))
            .Must(HaveDifferentNames)
            .WithErrorCode(this.ErrorCode(titles, Distinct, Names))
            .Must(HaveDifferentPriorities)
            .WithErrorCode(this.ErrorCode(titles, Distinct, Priorities));

        this.RuleForEach(titles)
            .SetValidator(titleValidator);
    }
}
