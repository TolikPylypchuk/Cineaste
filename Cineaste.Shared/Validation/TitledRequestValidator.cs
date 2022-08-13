namespace Cineaste.Shared.Validation;

using System.Linq.Expressions;

using Cineaste.Shared.Models;

public abstract class TitledRequestValidator<T> : CineasteValidator<T>
    where T : IValidatable<T>, ITitledRequest
{
    private const string Names = nameof(Names);
    private const string Priorities = nameof(Priorities);

    private static readonly IValidator<TitleRequest> titleValidator = TitleRequest.CreateValidator();

    protected TitledRequestValidator(string errorCodePrefix)
        : base(errorCodePrefix)
    {
        this.AddRulesForTitles(req => req.Titles);
        this.AddRulesForTitles(req => req.OriginalTitles);
    }

    private void AddRulesForTitles(Expression<Func<T, IEnumerable<TitleRequest>>> titles)
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

    private static bool HaveDifferentNames(IEnumerable<TitleRequest> titles) =>
        titles.DistinctBy(title => title.Name).Count() == titles.Count();

    private static bool HaveDifferentPriorities(IEnumerable<TitleRequest> titles) =>
        titles.DistinctBy(title => title.Priority).Count() == titles.Count();
}
