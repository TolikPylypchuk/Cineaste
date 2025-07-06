using System.Linq.Expressions;

using Cineaste.Shared.Models;
using Cineaste.Shared.Validation.Shared;

namespace Cineaste.Shared.Validation;

public abstract class TitledRequestValidator<T> : CineasteValidator<T>
    where T : IValidatable<T>, ITitledRequest
{
    private const string Names = nameof(Names);
    private const string Priorities = nameof(Priorities);

    private static readonly TitleRequestValidator TitlesValidator = new(nameof(ITitledRequest.Titles));
    private static readonly TitleRequestValidator OriginalTitlesValidator = new(nameof(ITitledRequest.OriginalTitles));

    protected TitledRequestValidator(string errorCodePrefix, bool mandatoryTitles = true)
        : base(errorCodePrefix)
    {
        this.AddRulesForTitles(req => req.Titles, TitlesValidator, mandatoryTitles);
        this.AddRulesForTitles(req => req.OriginalTitles, OriginalTitlesValidator, mandatoryTitles);
    }

    private void AddRulesForTitles(
        Expression<Func<T, IEnumerable<TitleRequest>>> titles,
        IValidator<TitleRequest> validator,
        bool mandatory)
    {
        if (mandatory)
        {
            this.RuleFor(titles)
                .NotEmpty()
                .WithErrorCode(this.ErrorCode(titles, Empty));
        }

        this.RuleFor(titles)
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
