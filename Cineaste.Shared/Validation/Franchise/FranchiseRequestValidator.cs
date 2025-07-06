namespace Cineaste.Shared.Validation.Franchise;

public sealed class FranchiseRequestValidator : TitledRequestValidator<FranchiseRequest>
{
    public FranchiseRequestValidator()
        : base("Franchise", mandatoryTitles: false)
    {
        this.RuleFor(req => new { req.ShowTitles, req.Titles, req.OriginalTitles })
            .Must(x => x.ShowTitles.Implies(x.Titles.Count != 0 && x.OriginalTitles.Count != 0))
            .WithErrorCode(this.ErrorCode(req => req.ShowTitles, Invalid));

        this.RuleForEach(req => req.Items)
            .Must(item => Enum.IsDefined(item.Type))
            .WithErrorCode(this.ErrorCode(req => req.Items, nameof(FranchiseItemModel.Type), Invalid));

        this.RuleFor(req => req.Items)
            .Must(items => items
                .Select(item => item.SequenceNumber)
                .OrderBy(num => num)
                .Select((num, index) => num == index + 1)
                .All(isValid => isValid))
            .WithErrorCode(this.ErrorCode(req => req.Items, Sequence, Invalid));
    }
}
