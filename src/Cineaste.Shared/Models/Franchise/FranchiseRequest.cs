using Cineaste.Shared.Validation.Franchise;

namespace Cineaste.Shared.Models.Franchise;

public record FranchiseRequest(
    Guid ListId,
    ImmutableValueList<TitleRequest> Titles,
    ImmutableValueList<TitleRequest> OriginalTitles,
    ImmutableValueList<FranchiseItemRequest> Items,
    Guid KindId,
    FranchiseKindSource KindSource,
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
