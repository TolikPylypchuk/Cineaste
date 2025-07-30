namespace Cineaste.Client.FormModels;

public sealed record FranchiseFormComponent(
    Guid Id,
    FranchiseItemType Type,
    string Title,
    int? StartYear,
    int? EndYear,
    int SequenceNumber,
    int? DisplayNumber);
