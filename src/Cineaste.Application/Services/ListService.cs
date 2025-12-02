using System.Linq.Expressions;

namespace Cineaste.Application.Services;

public sealed class ListService(CineasteDbContext dbContext, ILogger<ListService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<ListService> logger = logger;

    public async Task<ListModel> GetList(Id<CineasteList> listId, CancellationToken token)
    {
        logger.LogDebug("Getting the list");

        var list = await dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .AsSplitQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw this.ListNotFound(listId);

        return list.ToListModel();
    }

    public async Task<OffsettableData<ListItemModel>> GetListItems(
        Id<CineasteList> listId,
        int offset,
        int size,
        CancellationToken token)
    {
        logger.LogDebug("Getting the list items");

        var list = await dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw this.ListNotFound(listId);

        int totalItems = await dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Where(item => item.IsShown)
            .CountAsync(token);

        if (size == 0)
        {
            return new([], new(offset, size, totalItems));
        }

        var items = await dbContext.ListItems
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
        logger.LogDebug("Getting standalone list items");

        var list = await dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw this.ListNotFound(listId);

        var items = await dbContext.ListItems
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
        logger.LogDebug("Getting the list item by ID {Id}", id);

        var list = await dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw this.ListNotFound(listId);

        var movieId = Id.For<Movie>(id);
        var seriesId = Id.For<Series>(id);
        var franchiseId = Id.For<Franchise>(id);

        var item = await dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Where(item =>
                item.Movie!.Id == movieId ||
                item.Series!.Id == seriesId ||
                item.Franchise!.Id == franchiseId)
            .IncludeRelationships()
            .SingleOrDefaultAsync(token)
            ?? throw new NotFoundException(Resources.ListItem, $"Could not find list item with ID {id}");

        return item.ToListItemModel();
    }

    public async Task<ListItemModel> GetListItemByParentFranchise(
        Id<CineasteList> listId,
        Guid parentFranchiseId,
        int sequenceNumber,
        CancellationToken token)
    {
        logger.LogDebug(
            "Getting the list item by parent franchise ID {ParentFranchiseId} and sequence number {SequenceNumber}",
            parentFranchiseId,
            sequenceNumber);

        var list = await dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw this.ListNotFound(listId);

        var parentFranchiseStrongId = Id.For<Franchise>(parentFranchiseId);

        var parentFranchise = await dbContext.Franchises
            .Where(franchise => franchise.Id == parentFranchiseStrongId)
            .Where(franchise => franchise.ListItem!.List.Id == list.Id)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Movie)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Series)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Franchise)
            .SingleOrDefaultAsync(token)
            ?? throw new NotFoundException(Resources.Franchise, $"Franchise with ID {parentFranchiseId} not found");

        var childItem = parentFranchise.Children
            .FirstOrDefault(item => item.SequenceNumber == sequenceNumber)
            ?? throw new NotFoundException(
                Resources.ListItem,
                $"List item not found by parent franchise ID {parentFranchiseId} and sequence number {sequenceNumber}");

        var itemIdPredicate = childItem.Select<Expression<Func<ListItem, bool>>>(
            movie => item => item.Movie!.Id == movie.Id,
            series => item => item.Series!.Id == series.Id,
            franchise => item => item.Franchise!.Id == franchise.Id);

        var item = await dbContext.ListItems
            .Where(item => item.List.Id == list!.Id)
            .Where(itemIdPredicate)
            .IncludeRelationships()
            .SingleOrDefaultAsync(token)
            ?? throw this.ListItemNotFound(childItem);

        return item.ToListItemModel();
    }

    private NotFoundException ListNotFound(Id<CineasteList> listId) =>
        new(Resources.List, $"Could not find the list with ID {listId}");

    private NotFoundException ListItemNotFound(FranchiseItem item)
    {
        var itemIdValue = item.Select(
            movie => movie.Id.Value,
            series => series.Id.Value,
            franchise => franchise.Id.Value);

        return new NotFoundException(Resources.ListItem, $"Could not find list item with ID {itemIdValue}");
    }
}

file static class Extensions
{
    public static IQueryable<ListItem> IncludeRelationships(this IQueryable<ListItem> items) =>
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
