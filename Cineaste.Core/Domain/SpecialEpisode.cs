namespace Cineaste.Core.Domain;

public sealed class SpecialEpisode : DomainObject
{
    public int Month { get; set; }
    public int Year { get; set; }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public string Channel { get; set; } = String.Empty;

    public int SequenceNumber { get; set; }

    public string? RottenTomatoesLink { get; set; }
    public string? PosterUrl { get; set; }

    public int SeriesId { get; set; }

    public Series Series { get; set; } = null!;

    public List<Title> Titles { get; set; } = new();
}
