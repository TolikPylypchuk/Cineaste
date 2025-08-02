namespace Cineaste.Shared.Models.List;

public sealed record ListItemModel(
    Guid Id,
    ListItemType Type,
    string Title,
    string OriginalTitle,
    int? StartYear,
    int? EndYear,
    string Color,
    int ListSequenceNumber,
    ListFranchiseItemModel? FranchiseItem) : IComparable<ListItemModel>
{
    public int CompareTo(ListItemModel? other) =>
        other is not null ? this.ListSequenceNumber.CompareTo(other.ListSequenceNumber) : 1;
}

public enum ListItemType { Movie, Series, Franchise }

public sealed record ListFranchiseItemModel(
    Guid FranchiseId,
    int SequenceNumber,
    int? DisplayNumber,
    bool IsLooselyConnected);
