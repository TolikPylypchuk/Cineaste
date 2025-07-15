namespace Cineaste.Services;

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
            .AsSplitQuery()
            .SingleOrDefaultAsync()
            ?? throw this.NotFound();

        await dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.Titles)
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.FranchiseItem)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Titles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.Titles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.Periods)
            .Include(item => item.Series)
                .ThenInclude(series => series!.SpecialEpisodes)
                    .ThenInclude(episode => episode.Titles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.FranchiseItem)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.Titles)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.Children)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.FranchiseItem)
            .OrderBy(item => item.SequenceNumber)
            .AsSplitQuery()
            .ToListAsync();

        return list.ToListModel();
    }

    private NotFoundException NotFound() =>
        new(Resources.List, $"Could not find the list");
}
