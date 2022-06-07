namespace Cineaste.Server.Services;

using System.Globalization;

[AutoConstructor]
public sealed partial class ListService : IListService
{
    private readonly CineasteDbContext dbContext;
    private readonly IListMapper mapper;
    private readonly ILogger<ListService> logger;

    public async Task<List<SimpleListModel>> GetAllLists()
    {
        logger.LogDebug("Getting all lists");

        var lists = await dbContext.Lists
            .Select(list => new { list.Id, list.Name, list.Handle })
            .ToListAsync();

        return lists
            .OrderBy(list => list.Name)
            .Select(list => new SimpleListModel(list.Id.Value, list.Name, list.Handle))
            .ToList();
    }

    public async Task<ListModel> GetList(string handle)
    {
        logger.LogDebug("Getting the list with handle {Handle}", handle);

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
            .SingleOrDefaultAsync(list => list.Handle == handle);

        return list is not null
            ? this.mapper.MapToListModel(list)
            : throw this.NotFound(handle);
    }

    public async Task<SimpleListModel> CreateList(CreateListRequest request)
    {
        logger.LogDebug("Creating a list with handle {Handle}", request.Handle);

        var list = new CineasteList(
            Id.Create<CineasteList>(),
            request.Name,
            request.Handle,
            new ListConfiguration(
                Id.Create<ListConfiguration>(),
                CultureInfo.GetCultureInfo(request.Culture),
                request.DefaultSeasonTitle,
                request.DefaultSeasonOriginalTitle,
                ListSortingConfiguration.CreateDefault()));

        this.dbContext.Lists.Add(list);
        await this.dbContext.SaveChangesAsync();

        return new SimpleListModel(list.Id.Value, list.Name, list.Handle);
    }

    private Exception NotFound(string? handle) =>
        new NotFoundException(
            "NotFound.List",
            $"Could not find a list with handle {handle}",
            "Resource.List",
            new Dictionary<string, object?> { ["handle"] = handle });
}
