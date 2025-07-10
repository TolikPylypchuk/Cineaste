namespace Cineaste.Client.FormModels;

public sealed record FranchiseFormComponent(
    Guid Id,
    FranchiseItemType Type,
    string Title,
    int SequenceNumber,
    bool ShouldDisplayNumber);
