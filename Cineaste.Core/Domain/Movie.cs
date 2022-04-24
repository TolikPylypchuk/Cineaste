namespace Cineaste.Core.Domain;

public sealed class Movie : DomainObject
{
    public int Year { get; set; }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; } = true;

    public string? ImdbId { get; set; }
    public string? RottenTomatoesLink { get; set; }
    public string? PosterUrl { get; set; }

    public Kind Kind { get; set; } = null!;

    public FranchiseItem? FranchiseItem { get; set; }

    public List<Title> Titles { get; set; } = new();

    public HashSet<Tag> Tags { get; set; } = new();

    public MovieSeriesList OwnerList { get; set; } = null!;

    public Title Title =>
        this.Titles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    public Title OriginalTitle =>
        this.Titles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    public override string ToString() =>
        $"Movie #{this.Id}: {Title.ToString(this.Titles)} ({this.Year})";
}
