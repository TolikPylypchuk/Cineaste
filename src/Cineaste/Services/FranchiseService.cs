using Cineaste.Core.Domain;

namespace Cineaste.Services;

public sealed class FranchiseService(CineasteDbContext dbContext, ILogger<FranchiseService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly ILogger<FranchiseService> logger = logger;

    public async Task<FranchiseModel> GetFranchise(Id<Franchise> id)
    {
        this.logger.LogDebug("Getting the franchise with ID: {Id}", id.Value);

        var franchise = await this.FindFranchise(id);
        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> AddFranchise(Validated<FranchiseRequest> request)
    {
        this.logger.LogDebug("Creating a new franchise");

        var list = await this.FindList(request.Value.ListId);
        var franchise = await this.MapToFranchise(request);

        list.AddFranchise(franchise);
        dbContext.Franchises.Add(franchise);

        list.SortItems();

        await dbContext.SaveChangesAsync();

        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> UpdateFranchise(Id<Franchise> id, Validated<FranchiseRequest> request)
    {
        this.logger.LogDebug("Updating the franchise with ID: {Id}", id.Value);

        var franchise = await this.FindFranchise(id);
        var list = await this.FindList(request.Value.ListId);

        if (!list.ContainsFranchise(franchise))
        {
            throw this.FranchiseDoesNotBelongToList(id, list.Id);
        }

        var (movies, series, franchises) = await this.GetAllItems(request.Value);

        franchise.Update(request, movies, series, franchises);

        list.SortItems();
        franchise.ListItem?.SetProperties(franchise);

        await dbContext.SaveChangesAsync();

        return franchise.ToFranchiseModel();
    }

    public async Task RemoveFranchise(Id<Franchise> id)
    {
        this.logger.LogDebug("Deleting the franchise with ID: {Id}", id.Value);

        var franchise = await this.dbContext.Franchises
            .Where(franchise => franchise.Id == id)
            .Include(franchise => franchise.ListItem)
            .SingleOrDefaultAsync()
            ?? throw this.NotFound(id);

        var listItem = franchise.ListItem!;

        var list = await this.FindList(listItem.List.Id.Value);
        list.RemoveItem(listItem);
        list.SortItems();

        dbContext.ListItems.Remove(listItem);

        this.dbContext.Franchises.Remove(franchise);
        await this.dbContext.SaveChangesAsync();
    }

    private async Task<Franchise> FindFranchise(Id<Franchise> id)
    {
        var franchise = await this.dbContext.Franchises
            .Include(franchise => franchise.Titles)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Movie!.Titles)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Series!.Titles)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Series!.Seasons)
                    .ThenInclude(season => season!.Periods)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Series!.SpecialEpisodes)
            .Include(franchise => franchise.Children)
                .ThenInclude(item => item.Franchise!.Titles)
            .Include(franchise => franchise.FranchiseItem)
                .ThenInclude(item => item!.ParentFranchise)
                    .ThenInclude(franchise => franchise.Children)
            .AsSplitQuery()
            .SingleOrDefaultAsync(franchise => franchise.Id == id);

        return franchise is not null ? franchise : throw this.NotFound(id);
    }

    private async Task<CineasteList> FindList(Guid id)
    {
        var listId = Id.For<CineasteList>(id);

        var list = await this.dbContext.Lists
            .Include(list => list.Configuration)
            .Include(list => list.Items)
                .ThenInclude(item => item.Franchise)
                    .ThenInclude(franchise => franchise!.Titles)
            .AsSplitQuery()
            .SingleOrDefaultAsync(list => list.Id == listId);

        return list is not null
            ? list
            : throw this.NotFound(listId);
    }

    private async Task<Franchise> MapToFranchise(Validated<FranchiseRequest> request)
    {
        var (movies, series, franchises) = await this.GetAllItems(request.Value);

        return request.ToFranchise(
            Id.Create<Franchise>(),
            movies.ToDictionary(movie => movie.Id, movie => movie),
            series.ToDictionary(series => series.Id, series => series),
            franchises.ToDictionary(franchise => franchise.Id, franchise => franchise));
    }

    private async Task<(List<Movie>, List<Series>, List<Franchise>)> GetAllItems(FranchiseRequest request)
    {
        var (movies, missingMovieIds) = await this.GetItems<Movie>(request, FranchiseItemType.Movie);
        var (series, missingSeriesIds) = await this.GetItems<Series>(request, FranchiseItemType.Series);
        var (franchises, missingFranchiseIds) = await this.GetItems<Franchise>(request, FranchiseItemType.Franchise);

        if (missingMovieIds.Any() || missingSeriesIds.Any() || missingFranchiseIds.Any())
        {
            throw this.NotFound(missingMovieIds, missingSeriesIds, missingFranchiseIds);
        }

        return (movies, series, franchises);
    }

    private async Task<(List<T>, IReadOnlySet<Id<T>>)> GetItems<T>(FranchiseRequest request, FranchiseItemType itemType)
        where T : FranchiseItemEntity<T>
    {
        var ids = request.Items
            .Where(item => item.Type == itemType)
            .Select(item => Id.For<T>(item.Id))
            .ToImmutableHashSet();

        var items = ids.IsEmpty()
            ? []
            : await this.dbContext.Set<T>()
                .Where(item => ids.Contains(item.Id))
                .Include(item => item.FranchiseItem)
                .ToListAsync();

        var missingIds = ids.Except(items.Select(item => item.Id)).ToImmutableSortedSet();

        return (items, missingIds);
    }

    private CineasteException NotFound(Id<Franchise> id) =>
        new NotFoundException(Resources.Franchise, $"Could not find a franchise with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<CineasteList> id) =>
        new NotFoundException(Resources.List, $"Could not find a list with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(
        IEnumerable<Id<Movie>> movieIds,
        IEnumerable<Id<Series>> seriesIds,
        IEnumerable<Id<Franchise>> franchiseIds) =>
        new NotFoundException(Resources.FranchiseItems, $"Could not find franchise items")
            .WithProperty(movieIds)
            .WithProperty(seriesIds)
            .WithProperty(franchiseIds);

    private CineasteException FranchiseDoesNotBelongToList(Id<Franchise> franchiseId, Id<CineasteList> listId) =>
        new InvalidInputException(
            $"{Resources.Franchise}.WrongList",
            $"Franchise with ID {franchiseId.Value} does not belong to list with ID {listId}")
            .WithProperty(franchiseId)
            .WithProperty(listId);
}
