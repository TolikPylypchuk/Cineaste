namespace Cineaste.Shared.Models.List;

public sealed record ListItemModel(
    Guid Id,
    ListItemType Type,
    bool ShouldBeShown,
    string DisplayNumber,
    string Title,
    string OriginalTitle,
    int StartYear,
    int EndYear,
    string Color,
    ListFranchiseItemModel? FranchiseItem)
{
    public string Years =>
        this.Type == ListItemType.Franchise
            ? String.Empty
            : this.StartYear == this.EndYear ? this.StartYear.ToString() : $"{this.StartYear}-{this.EndYear}";
}

public enum ListItemType { Movie, Series, Franchise }

public sealed record ListFranchiseItemModel(Guid FranchiseId, int SequenceNumber);
