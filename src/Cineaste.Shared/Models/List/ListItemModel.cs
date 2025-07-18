namespace Cineaste.Shared.Models.List;

public sealed record ListItemModel(
    Guid Id,
    ListItemType Type,
    string Title,
    string OriginalTitle,
    int StartYear,
    int EndYear,
    string Color,
    ListFranchiseItemModel? FranchiseItem);

public enum ListItemType { Movie, Series, Franchise }

public sealed record ListFranchiseItemModel(
    Guid FranchiseId,
    int SequenceNumber,
    int? DisplayNumber,
    bool IsLooselyConnected);
