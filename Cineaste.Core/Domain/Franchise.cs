namespace Cineaste.Core.Domain;

public sealed class Franchise : DomainObject
{
    public bool ShowTitles { get; set; }
    public bool IsLooselyConnected { get; set; }
    public bool MergeDisplayNumbers { get; set; }

    public string? PosterUrl { get; set; }

    public FranchiseItem? ParentFranchiseItem { get; set; }

    public List<FranchiseItem> Children { get; set; } = new();

    public List<Title> Titles { get; set; } = new();

    public CineasteList OwnerList { get; set; } = null!;
}
