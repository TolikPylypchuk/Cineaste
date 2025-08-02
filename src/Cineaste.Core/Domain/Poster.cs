namespace Cineaste.Core.Domain;

public interface IPoster
{
    byte[] Data { get; }

    string ContentType { get; }
}

public abstract class Poster<TPoster> : Entity<TPoster>, IPoster
    where TPoster : Poster<TPoster>
{
    public byte[] Data { get; private set; }

    public string ContentType { get; private set; }

    public Poster(Id<TPoster> id, byte[] data, string contentType)
        : base(id)
    {
        this.Data = Require.NotEmpty(data);
        this.ContentType = Require.NotEmpty(contentType);
    }

    protected Poster(Id<TPoster> id)
        : base(id)
    {
        this.Data = [];
        this.ContentType = String.Empty;
    }
}

public sealed class MoviePoster : Poster<MoviePoster>
{
    public Movie Movie { get; private set; }

    public MoviePoster(Id<MoviePoster> id, Movie movie, byte[] data, string contentType)
        : base(id, data, contentType) =>
        this.Movie = Require.NotNull(movie);

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private MoviePoster(Id<MoviePoster> id)
        : base(id) =>
        this.Movie = null!;
}

public sealed class SeriesPoster : Poster<SeriesPoster>
{
    public Series Series { get; private set; }

    public SeriesPoster(Id<SeriesPoster> id, Series series, byte[] data, string contentType)
        : base(id, data, contentType) =>
        this.Series = Require.NotNull(series);

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private SeriesPoster(Id<SeriesPoster> id)
        : base(id) =>
        this.Series = null!;
}

public sealed class FranchisePoster : Poster<FranchisePoster>
{
    public Franchise Franchise { get; private set; }

    public FranchisePoster(Id<FranchisePoster> id, Franchise franchise, byte[] data, string contentType)
        : base(id, data, contentType) =>
        this.Franchise = Require.NotNull(franchise);

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private FranchisePoster(Id<FranchisePoster> id)
        : base(id) =>
        this.Franchise = null!;
}

public sealed class SeasonPoster : Poster<SeasonPoster>
{
    public Period Period { get; private set; }

    public SeasonPoster(Id<SeasonPoster> id, Period period, byte[] data, string contentType)
        : base(id, data, contentType) =>
        this.Period = Require.NotNull(period);

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private SeasonPoster(Id<SeasonPoster> id)
        : base(id) =>
        this.Period = null!;
}

public sealed class SpecialEpisodePoster : Poster<SpecialEpisodePoster>
{
    public SpecialEpisode SpecialEpisode { get; private set; }

    public SpecialEpisodePoster(Id<SpecialEpisodePoster> id, SpecialEpisode episode, byte[] data, string contentType)
        : base(id, data, contentType) =>
        this.SpecialEpisode = Require.NotNull(episode);

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private SpecialEpisodePoster(Id<SpecialEpisodePoster> id)
        : base(id) =>
        this.SpecialEpisode = null!;
}
