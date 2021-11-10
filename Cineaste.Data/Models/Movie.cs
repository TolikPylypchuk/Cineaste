namespace Cineaste.Data.Models;

[Table("Movies")]
public sealed class Movie : EntityBase
{
    public int Year { get; set; } = 2000;

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; } = true;

    public string? ImdbLink { get; set; }
    public string? RottenTomatoesLink { get; set; }
    public string? PosterUrl { get; set; }

    public int KindId { get; set; }

    [Write(false)]
    public Kind Kind { get; set; } = null!;

    [Write(false)]
    public FranchiseEntry? Entry { get; set; }

    [Write(false)]
    public List<Title> Titles { get; set; } = new();

    [Write(false)]
    public HashSet<Tag> Tags { get; set; } = new();

    [Computed]
    public Title Title =>
        this.Titles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    [Computed]
    public Title OriginalTitle =>
        this.Titles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    public override string ToString() =>
        $"Movie #{this.Id}: {Title.ToString(this.Titles)} ({this.Year})";
}
