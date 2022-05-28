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
            .Select(list => new { list.Id, list.Name })
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
            .SingleOrDefaultAsync(list => list.Id == listId);

        return list is not null
            ? new ListModel(
                list.Id.Value,
                list.Name,
                this.ToConfigurationModel(list.Configuration),
                list.Movies.Select(this.ToListItemModel).ToList(),
                list.Series.Select(this.ToListItemModel).ToList(),
                list.Franchises.Select(this.ToListItemModel).ToList(),
                list.MovieKinds.Select(this.ToListKindModel).ToList(),
                list.SeriesKinds.Select(this.ToListKindModel).ToList())
            : null;
    }

    private ListConfigurationModel ToConfigurationModel(ListConfiguration config) =>
        new(
            config.Culture.ToString(),
            config.DefaultSeasonTitle,
            config.DefaultSeasonOriginalTitle,
            config.SortingConfiguration.DefaultFirstSortOrder,
            config.SortingConfiguration.DefaultFirstSortDirection,
            config.SortingConfiguration.DefaultSecondSortOrder,
            config.SortingConfiguration.DefaultSecondSortDirection);

    private ListItemModel ToListItemModel(Movie movie) =>
        new(
            movie.Id.Value,
            ListItemType.Movie,
            true,
            movie.FranchiseItem.GetDisplayNumber(),
            movie.Title.Name,
            movie.OriginalTitle.Name,
            movie.Year,
            movie.Year,
            movie.GetActiveColor().HexValue,
            movie.FranchiseItem is not null ? movie.FranchiseItem.ParentFranchise.Id.Value : null);

    private ListItemModel ToListItemModel(Series series) =>
        new(
            series.Id.Value,
            ListItemType.Series,
            true,
            series.FranchiseItem.GetDisplayNumber(),
            series.Title.Name,
            series.OriginalTitle.Name,
            series.StartYear,
            series.EndYear,
            series.GetActiveColor().HexValue,
            series.FranchiseItem is not null ? series.FranchiseItem.ParentFranchise.Id.Value : null);

    private ListItemModel ToListItemModel(Franchise franchise) =>
        new(
            franchise.Id.Value,
            ListItemType.Franchise,
            franchise.ShowTitles,
            franchise.FranchiseItem.GetDisplayNumber(),
            franchise.ShowTitles && franchise.Title is not null
                ? $"{franchise.Title.Name}:"
                : String.Empty,
            franchise.ShowTitles && franchise.OriginalTitle is not null
                ? $"{franchise.OriginalTitle.Name}:"
                : String.Empty,
            franchise.GetFirstChild()?.GetStartYear() ?? 0,
            franchise.GetLastChild()?.GetEndYear() ?? 0,
            franchise.GetActiveColor()?.HexValue ?? String.Empty,
            franchise.FranchiseItem is not null ? franchise.FranchiseItem.ParentFranchise.Id.Value : null);

    private ListKindModel ToListKindModel<TKind>(Kind<TKind> kind)
        where TKind : Kind<TKind> =>
        new(
            kind.Id.Value,
            kind.Name,
            kind.WatchedColor.HexValue,
            kind.NotWatchedColor.HexValue,
            kind.NotReleasedColor.HexValue,
            kind is MovieKind ? ListKindTarget.Movie : ListKindTarget.Series);
}
