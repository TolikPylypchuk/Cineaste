namespace Cineaste.Core.Domain;

public sealed class CineasteList : Entity<CineasteList>
{
    private readonly List<Movie> movies;
    private readonly List<Series> series;
    private readonly List<Franchise> franchises;

    private readonly List<MovieKind> movieKinds;
    private readonly List<SeriesKind> seriesKinds;
    private readonly List<Tag> tags;

    public ListConfiguration Configuration { get; private set; }

    public IReadOnlyCollection<Movie> Movies =>
        this.movies.AsReadOnly();

    public IReadOnlyCollection<Series> Series =>
        this.series.AsReadOnly();

    public IReadOnlyCollection<Franchise> Franchises =>
        this.franchises.AsReadOnly();

    public IReadOnlyCollection<MovieKind> MovieKinds =>
        this.movieKinds.AsReadOnly();

    public IReadOnlyCollection<SeriesKind> SeriesKinds =>
        this.seriesKinds.AsReadOnly();

    public IReadOnlyCollection<Tag> Tags =>
        this.tags.AsReadOnly();

    public CineasteList(Id<CineasteList> id, ListConfiguration configuration)
        : base(id)
    {
        this.Configuration = Require.NotNull(configuration);

        this.movies = new();
        this.series = new();
        this.franchises = new();

        this.movieKinds = new();
        this.seriesKinds = new();
        this.tags = new();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private CineasteList(Id<CineasteList> id)
        : base(id)
    {
        this.Configuration = null!;

        this.movies = new();
        this.series = new();
        this.franchises = new();

        this.movieKinds = new();
        this.seriesKinds = new();
        this.tags = new();
    }

    public void AddMovie(Movie movie) =>
        this.movies.Add(movie);

    public void RemoveMovie(Movie movie) =>
        this.movies.Remove(movie);

    public void AddSeries(Series series) =>
        this.series.Add(series);

    public void RemoveSeries(Series series) =>
        this.series.Remove(series);

    public void AddFranchise(Franchise franchise) =>
        this.franchises.Add(franchise);

    public void RemoveFranchise(Franchise franchise) =>
        this.franchises.Remove(franchise);

    public void AddMovieKind(MovieKind movieKind) =>
        this.movieKinds.Add(movieKind);

    public void RemoveMovieKind(MovieKind movieKind) =>
        this.movieKinds.Remove(movieKind);

    public void AddSeriesKind(SeriesKind seriesKind) =>
        this.seriesKinds.Add(seriesKind);

    public void RemoveSeriesKind(SeriesKind seriesKind) =>
        this.seriesKinds.Remove(seriesKind);

    public void AddTag(Tag tag) =>
        this.tags.Add(tag);

    public void RemoveTag(Tag tag) =>
        this.tags.Remove(tag);
}
