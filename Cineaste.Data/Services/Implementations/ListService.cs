namespace Cineaste.Data.Services.Implementations;

internal class ListService : ServiceBase, IListService
{
    public ListService(string file)
        : base(file)
    { }

    public DataList GetList(IEnumerable<Kind> kinds, IEnumerable<Tag> tags) =>
        this.WithTransaction((connection, transaction) => this.GetList(kinds, tags, connection, transaction));

    private DataList GetList(
        IEnumerable<Kind> kinds,
        IEnumerable<Tag> tags,
        IDbConnection connection,
        IDbTransaction transaction)
    {
        this.Log().Debug("Getting the entire list of movies, series and franchises");

        var titles = connection.GetAll<Title>(transaction).ToList();
        var entries = connection.GetAll<FranchiseEntry>(transaction).ToList();

        var seasons = connection.GetAll<Season>(transaction).ToList();
        var periods = connection.GetAll<Period>(transaction).ToList();
        var specialEpisodes = connection.GetAll<SpecialEpisode>(transaction).ToList();

        var movies = connection.GetAll<Movie>(transaction).ToList();
        var series = connection.GetAll<Series>(transaction).ToList();
        var franchises = connection.GetAll<Franchise>(transaction).ToList();

        var movieTags = connection.GetAll<MovieTag>(transaction).ToList();
        var seriesTags = connection.GetAll<SeriesTag>(transaction).ToList();

        var tagsById = tags.ToDictionary(tag => tag.Id, tag => tag);

        return new DataList(
            movies
                .Join(kinds)
                .Join(titles)
                .Join(entries)
                .Join(movieTags, tagsById)
                .ToList(),
            series
                .Join(kinds)
                .Join(titles)
                .Join(seasons.Join(periods).Join(titles))
                .Join(specialEpisodes.Join(titles))
                .Join(entries)
                .Join(seriesTags, tagsById)
                .ToList(),
            franchises
                .Join(titles)
                .Join(entries)
                .ToList());
    }
}
