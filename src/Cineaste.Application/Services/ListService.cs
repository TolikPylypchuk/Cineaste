using System.Linq.Expressions;

namespace Cineaste.Application.Services;

public sealed partial class ListService(CineasteDbContext dbContext, ILogger<ListService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<ListService> logger = logger;

    public async Task<ListModel> GetList(Id<CineasteList> listId, CancellationToken token)
    {
        this.LogGetList(listId);

        var list = await this.dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .AsSplitQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw new ListNotFoundException(listId);

        return list.ToListModel();
    }

    public async Task<OffsettableData<ListItemModel>> GetListItems(
        Id<CineasteList> listId,
        int offset,
        int size,
        CancellationToken token)
    {
        this.LogGetListItems(listId, offset, size);

        var list = await this.dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .AsSplitQuery()
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw new ListNotFoundException(listId);

        int totalItems = await this.dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Where(item => item.IsShown)
            .CountAsync(token);

        if (size == 0)
        {
            return new([], new(offset, size, totalItems));
        }

        var items = await this.dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Where(item => item.IsShown)
            .IncludeRelationships()
            .OrderBy(item => item.SequenceNumber)
            .Skip(offset)
            .Take(size)
            .AsSplitQuery()
            .ToListAsync(token);

        return new(
            [.. items.Select(item => item.ToListItemModel())],
            new(offset, size, totalItems));
    }

    public async Task<List<ListItemModel>> GetStandaloneListItems(Id<CineasteList> listId, CancellationToken token)
    {
        this.LogGetStandaloneListItems(listId);

        var list = await this.dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw new ListNotFoundException(listId);

        var items = await this.dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Where(item => item.IsShown && item.IsStandalone)
            .IncludeRelationships()
            .OrderBy(item => item.SequenceNumber)
            .AsSplitQuery()
            .ToListAsync(token);

        return [.. items.Select(item => item.ToListItemModel())];
    }

    public async Task<ListItemModel> GetListItem(Id<CineasteList> listId, Guid id, CancellationToken token)
    {
        this.LogGetListItem(listId, id);

        var list = await this.dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw new ListNotFoundException(listId);

        var movieId = Id.For<Movie>(id);
        var seriesId = Id.For<Series>(id);
        var franchiseId = Id.For<Franchise>(id);

        var item = await this.dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Where(item =>
                item.Movie!.Id == movieId ||
                item.Series!.Id == seriesId ||
                item.Franchise!.Id == franchiseId)
            .IncludeRelationships()
            .SingleOrDefaultAsync(token)
            ?? throw new ListItemNotFoundException(id);

        return item.ToListItemModel();
    }

    public async Task<ListItemModel> GetListItemByParentFranchise(
        Id<CineasteList> listId,
        Id<Franchise> parentFranchiseId,
        int sequenceNumber,
        CancellationToken token)
    {
        this.LogGetListItemByParentFranchise(listId, parentFranchiseId, sequenceNumber);

        var list = await this.dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw new ListNotFoundException(listId);

        var parentFranchise = await this.dbContext.Franchises
            .Where(franchise => franchise.Id == parentFranchiseId)
            .Where(franchise => franchise.ListItem!.List.Id == list.Id)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Movie)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Series)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Franchise)
            .SingleOrDefaultAsync(token)
            ?? throw new FranchiseNotFoundException(parentFranchiseId);

        var childItem = parentFranchise.Children
            .FirstOrDefault(item => item.SequenceNumber == sequenceNumber)
            ?? throw new FranchiseItemWithNumberNotFoundException(parentFranchiseId, sequenceNumber);

        var itemIdPredicate = childItem.Select<Expression<Func<ListItem, bool>>>(
            movie => item => item.Movie!.Id == movie.Id,
            series => item => item.Series!.Id == series.Id,
            franchise => item => item.Franchise!.Id == franchise.Id);

        var item = await this.dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Where(itemIdPredicate)
            .IncludeRelationships()
            .SingleOrDefaultAsync(token)
            ?? throw new FranchiseItemNotFoundException(childItem);

        return item.ToListItemModel();
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting list {ListId}")]
    private partial void LogGetList(Id<CineasteList> listId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Getting items of list {ListId} with offset {Offset} and size {Size}")]
    private partial void LogGetListItems(Id<CineasteList> listId, int offset, int size);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting standalone items of list {ListId}")]
    private partial void LogGetStandaloneListItems(Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting item {ItemId} of list {ListId}")]
    private partial void LogGetListItem(Id<CineasteList> listId, Guid itemId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Getting the item of list {ListId} by parent franchise {ParentFranchiseId} " +
            "and sequence number {SequenceNumber}")]
    private partial void LogGetListItemByParentFranchise(
        Id<CineasteList> listId,
        Id<Franchise> parentFranchiseId,
        int sequenceNumber);
}

file static class Extensions
{
    extension(IQueryable<ListItem> items)
    {
        public IQueryable<ListItem> IncludeRelationships() =>
            items
                .Include(item => item.Movie)
                    .ThenInclude(movie => movie!.FranchiseItem)
                        .ThenInclude(item => item!.ParentFranchise)
                .Include(item => item.Series)
                    .ThenInclude(series => series!.FranchiseItem)
                        .ThenInclude(item => item!.ParentFranchise)
                .Include(item => item.Franchise)
                    .ThenInclude(franchise => franchise!.FranchiseItem)
                        .ThenInclude(item => item!.ParentFranchise);
    }
}
