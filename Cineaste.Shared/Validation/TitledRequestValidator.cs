namespace Cineaste.Shared.Validation;

using System.Linq.Expressions;

using Cineaste.Shared.Models;
using Cineaste.Shared.Validation.Shared;

using FluentValidation;

public abstract class TitledRequestValidator<T> : CineasteValidator<T>
    where T : IValidatable<T>, ITitledRequest
{
    private const string Names = nameof(Names);
    private const string Priorities = nameof(Priorities);

    private static readonly TitleRequestValidator TitlesValidator = new(nameof(ITitledRequest.Titles));
    private static readonly TitleRequestValidator OriginalTitlesValidator = new(nameof(ITitledRequest.OriginalTitles));

    protected TitledRequestValidator(string errorCodePrefix)
        : base(errorCodePrefix)
    {
        this.AddRulesForTitles(req => req.Titles, TitlesValidator);
        this.AddRulesForTitles(req => req.OriginalTitles, OriginalTitlesValidator);
    }

    private void AddRulesForTitles(
        Expression<Func<T, IEnumerable<TitleRequest>>> titles,
        IValidator<TitleRequest> validator)
    {
        this.RuleFor(titles)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(titles, Empty))
            .Must(HaveDifferentNames)
            .WithErrorCode(this.ErrorCode(titles, Distinct, Names))
            .Must(HaveDifferentPriorities)
            .WithErrorCode(this.ErrorCode(titles, Distinct, Priorities));

        this.RuleForEach(titles)
            .SetValidator(validator);
    }

    private static bool HaveDifferentNames(IEnumerable<TitleRequest> titles) =>
        titles.DistinctBy(title => title.Name).Count() == titles.Count();

    private static bool HaveDifferentPriorities(IEnumerable<TitleRequest> titles) =>
        titles.DistinctBy(title => title.Priority).Count() == titles.Count();
}
