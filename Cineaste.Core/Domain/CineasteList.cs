namespace Cineaste.Core.Domain;

public sealed class CineasteList : Entity<CineasteList>
{
    private string name;

    private readonly List<Movie> movies;
    private readonly List<Series> series;
    private readonly List<Franchise> franchises;

    public string Name
    {
        get => this.name;

        [MemberNotNull(nameof(name))]
        set => this.name = Require.NotBlank(value);
    }

    public ListConfiguration Config { get; }

    public IReadOnlyCollection<Movie> Movies =>
        this.movies.AsReadOnly();

    public IReadOnlyCollection<Series> Series =>
        this.series.AsReadOnly();

    public IReadOnlyCollection<Franchise> Franchises =>
        this.franchises.AsReadOnly();

    public CineasteList(
        Id<CineasteList> id,
        string name,
        ListConfiguration config,
        IEnumerable<Movie> movies,
        IEnumerable<Series> series,
        IEnumerable<Franchise> franchises)
        : base(id)
    {
        this.Name = name;
        this.Config = Require.NotNull(config);

        this.movies = Require.NotNull(movies).ToList();
        this.series = Require.NotNull(series).ToList();
        this.franchises = Require.NotNull(franchises).ToList();
    }
}
