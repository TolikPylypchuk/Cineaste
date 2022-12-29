namespace Cineaste.Shared.Models.Franchise;

using Cineaste.Shared.Validation.Franchise;

public record FranchiseRequest(
    Guid ListId,
    ImmutableValueList<TitleRequest> Titles,
    ImmutableValueList<TitleRequest> OriginalTitles,
    ImmutableValueList<FranchiseItemRequest> Items,
    bool ShowTitles,
    bool IsLooselyConnected,
    bool ContinueNumbering) : IValidatable<FranchiseRequest>, ITitledRequest
{
    public static IValidator<FranchiseRequest> Validator { get; } = new FranchiseRequestValidator();
}

public record FranchiseItemRequest(
    Guid Id,
    FranchiseItemType Type,
    int SequenceNumber,
    bool ShouldDisplayNumber);
