namespace Cineaste.Server.Services;

[AutoConstructor]
public sealed partial class FranchiseService
{
    private readonly CineasteDbContext dbContext;
    private readonly ILogger<FranchiseService> logger;

    public async Task<FranchiseModel> GetFranchise(Id<Franchise> id)
    {
        this.logger.LogDebug("Getting the franchise with id: {Id}", id.Value);

        var franchise = await this.FindFranchise(id);
        return franchise.ToFranchiseModel();
    }

    public async Task<FranchiseModel> CreateFranchise(Validated<FranchiseRequest> request)
    {
        this.logger.LogDebug("Creating a new franchise");

        var list = await this.FindList(request.Value.ListId);
        var franchise = await this.MapToFranchise(request);

        list.AddFranchise(franchise);
        dbContext.Franchises.Add(franchise);

        await dbContext.SaveChangesAsync();

        return franchise.ToFranchiseModel();
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
            .AsSplitQuery()
            .SingleOrDefaultAsync(franchise => franchise.Id == id);

        return franchise is not null ? franchise : throw this.NotFound(id);
    }

    private async Task<CineasteList> FindList(Guid id)
    {
        var listId = Id.Create<CineasteList>(id);

        var list = await this.dbContext.Lists
            .Include(list => list.Franchises)
                .ThenInclude(franchise => franchise.Titles)
            .AsSplitQuery()
            .SingleOrDefaultAsync(list => list.Id == listId);

        if (list is null)
        {
            throw this.NotFound(listId);
        }

        return list;
    }

    private async Task<Franchise> MapToFranchise(Validated<FranchiseRequest> request)
    {
        var movieIds = request.Value
            .Items
            .Where(item => item.Type == FranchiseItemType.Movie)
            .Select(item => Id.Create<Movie>(item.Id))
            .ToHashSet();

        var seriesIds = request.Value
            .Items
            .Where(item => item.Type == FranchiseItemType.Series)
            .Select(item => Id.Create<Series>(item.Id))
            .ToHashSet();

        var franchiseIds = request.Value
            .Items
            .Where(item => item.Type == FranchiseItemType.Franchise)
            .Select(item => Id.Create<Franchise>(item.Id))
            .ToHashSet();

        var movies = movieIds.IsEmpty()
            ? new List<Movie>()
            : await this.dbContext.Movies
                .Where(movie => movieIds.Contains(movie.Id))
                .ToListAsync();

        var series = seriesIds.IsEmpty()
            ? new List<Series>()
            : await this.dbContext.Series
                .Where(series => seriesIds.Contains(series.Id))
                .ToListAsync();

        var franchises = franchiseIds.IsEmpty()
            ? new List<Franchise>()
            : await this.dbContext.Franchises
                .Where(franchise => franchiseIds.Contains(franchise.Id))
                .ToListAsync();

        var missingMovieIds = movieIds
            .Except(movies.Select(movie => movie.Id))
            .ToImmutableSortedSet();

        var missingSeriesIds = seriesIds
            .Except(series.Select(series => series.Id))
            .ToImmutableSortedSet();

        var missingFranchiseIds = franchiseIds
            .Except(franchises.Select(fracnhise => fracnhise.Id))
            .ToImmutableSortedSet();

        if (missingMovieIds.Any() || missingSeriesIds.Any() || missingFranchiseIds.Any())
        {
            throw this.NotFound(missingMovieIds, missingSeriesIds, missingFranchiseIds);
        }

        return request.ToFranchise(
            Id.CreateNew<Franchise>(),
            movies.ToDictionary(movie => movie.Id, movie => movie),
            series.ToDictionary(series => series.Id, series => series),
            franchises.ToDictionary(franchise => franchise.Id, franchise => franchise));
    }

    private Exception NotFound(Id<Franchise> id) =>
        new NotFoundException(Resources.Franchise, $"Could not find a franchise with id {id.Value}")
            .WithProperty(id);

    private Exception NotFound(Id<CineasteList> id) =>
        new NotFoundException(Resources.List, $"Could not find a list with id {id.Value}")
            .WithProperty(id);

    private Exception NotFound(
        IEnumerable<Id<Movie>> movieIds,
        IEnumerable<Id<Series>> seriesIds,
        IEnumerable<Id<Franchise>> franchiseIds) =>
        new NotFoundException(Resources.FranchiseItems, $"Could not find franchise items")
            .WithProperty(movieIds)
            .WithProperty(seriesIds)
            .WithProperty(franchiseIds);
}
