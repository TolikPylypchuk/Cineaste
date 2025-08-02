namespace Cineaste.Shared.Models.Shared;

public sealed record FranchiseItemInfoModel(
    Guid ParentFranchiseId,
    Guid RootFranchiseId,
    int SequenceNumber,
    int? DisplayNumber,
    bool IsFirstInFranchise,
    bool IsLastInFranchise,
    bool IsLooselyConnected);
