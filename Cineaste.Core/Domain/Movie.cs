namespace Cineaste.Core.Domain;

public sealed class Movie : DomainObject
{
    public int Year { get; set; }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; } = true;

    public string? ImdbId { get; set; }
    public string? RottenTomatoesLink { get; set; }
    public Poster? Poster { get; set; }

    public MovieKind Kind { get; set; } = null!;

    public FranchiseItem? FranchiseItem { get; set; }

    public List<Title> Titles { get; set; } = new();

    public HashSet<Tag> Tags { get; set; } = new();

    public CineasteList OwnerList { get; set; } = null!;
}
