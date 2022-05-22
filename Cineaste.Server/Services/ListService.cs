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

        var lists = await dbContext.Lists.ToListAsync();

        return lists
            .OrderBy(list => list.Name)
            .Select(list => new SimpleListModel(list.Name))
            .ToList();
    }
}
