namespace Cineaste.Core.Domain;

public sealed class Title : DomainObject
{
    public string Name { get; set; } = String.Empty;

    public int Priority { get; set; } = 1;

    public bool IsOriginal { get; set; }

    public Movie? Movie { get; set; }

    public Series? Series { get; set; }

    public Season? Season { get; set; }

    public SpecialEpisode? SpecialEpisode { get; set; }

    public Franchise? Franchise { get; set; }

    public static string ToString(IEnumerable<Title> titles)
    {
        var titlesAsList = titles.ToList();

        var sortedTitles = titlesAsList
            .Where(t => !t.IsOriginal)
            .OrderBy(t => t.Priority)
            .Select(t => t.Name)
            .Union(titlesAsList
                .Where(t => t.IsOriginal)
                .OrderBy(t => t.Priority)
                .Select(t => t.Name))
            .ToList();

        return sortedTitles.Count == 0
            ? String.Empty
            : sortedTitles.Aggregate((acc, item) => $"{acc}/{item}");
    }

    public override string ToString() =>
        $"Title #{this.Id}: {this.Name}";
}
