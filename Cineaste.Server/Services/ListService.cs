namespace Cineaste.Server.Services;

public sealed class ListService(CineasteDbContext dbContext, ILogger<ListService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<ListService> logger = logger;

    public async Task<ListModel> GetList()
    {
        logger.LogDebug("Getting the list");

        var list = await dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .Include(list => list.Movies)
                .ThenInclude(movie => movie.Titles)
            .Include(list => list.Movies)
                .ThenInclude(movie => movie.FranchiseItem)
            .Include(list => list.Series)
                .ThenInclude(series => series.Titles)
            .Include(list => list.Series)
                .ThenInclude(series => series.Seasons)
                    .ThenInclude(season => season.Titles)
            .Include(list => list.Series)
                .ThenInclude(series => series.Seasons)
                    .ThenInclude(season => season.Periods)
            .Include(list => list.Series)
                .ThenInclude(series => series.SpecialEpisodes)
                    .ThenInclude(episode => episode.Titles)
            .Include(list => list.Series)
                .ThenInclude(series => series.FranchiseItem)
            .Include(list => list.Franchises)
                .ThenInclude(franchise => franchise.Titles)
            .Include(list => list.Franchises)
                .ThenInclude(franchise => franchise.Children)
            .Include(list => list.Franchises)
                .ThenInclude(franchise => franchise.FranchiseItem)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        return list is not null
            ? list.ToListModel()
            : throw this.NotFound();
    }

    private CineasteException NotFound() =>
        new NotFoundException(Resources.List, $"Could not find the list");
}
