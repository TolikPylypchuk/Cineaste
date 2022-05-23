namespace Cineaste.Server.Services;

public sealed class ListService : IListService
{
    private readonly CineasteDbContext dbContext;
    private readonly ILogger<ListService> logger;

    public ListService(CineasteDbContext dbContext, ILogger<ListService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<List<SimpleListModel>> GetAllLists()
    {
        logger.LogDebug("Getting all lists");

        var lists = await dbContext.Lists
            .AsNoTracking()
            .ToListAsync();

        return lists
            .OrderBy(list => list.Name)
            .Select(list => new SimpleListModel(list.Id.Value, list.Name))
            .ToList();
    }

    public async Task<ListModel?> GetList(Guid id)
    {
        logger.LogDebug("Getting the list with ID {ID}", id);

        var listId = new Id<CineasteList>(id);

        var list = await dbContext.Lists
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
            .AsNoTracking()
            .SingleOrDefaultAsync(list => list.Id == listId);

        if (list is null)
        {
            return null;
        }

        return null;
    }
}
