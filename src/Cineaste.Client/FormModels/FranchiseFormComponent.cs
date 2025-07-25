namespace Cineaste.Client.FormModels;

public sealed record FranchiseFormComponent(
    Guid Id,
    FranchiseItemType Type,
    string Title,
    string Years,
    int SequenceNumber,
    int? DisplayNumber);
