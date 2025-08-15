using Cineaste.Services.Poster;

namespace Cineaste.Services;

public sealed class MovieService(
    CineasteDbContext dbContext,
    IPosterProvider posterProvider,
    ILogger<MovieService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly IPosterProvider posterProvider = posterProvider;
    private readonly ILogger<MovieService> logger = logger;

    public async Task<MovieModel> GetMovie(Id<Movie> id, CancellationToken token)
    {
        this.logger.LogDebug("Getting the movie with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, id, token);

        return movie.ToMovieModel();
    }

    public async Task<MovieModel> AddMovie(Validated<MovieRequest> request, CancellationToken token)
    {
        this.logger.LogDebug("Adding a new movie");

        var list = await this.FindList(token);
        var kind = this.FindKind(list, Id.For<MovieKind>(request.Value.KindId));

        var movie = request.ToMovie(Id.Create<Movie>(), kind);

        if (request.Value.ParentFranchiseId is Guid franchiseId)
        {
            var franchise = this.FindFranchise(list, Id.For<Franchise>(franchiseId));
            var franchiseItem = franchise.AttachMovie(movie, true);
            this.dbContext.FranchiseItems.Add(franchiseItem);
        }

        list.AddMovie(movie);
        list.SortItems();

        this.dbContext.Movies.Add(movie);
        await this.dbContext.SaveChangesAsync(token);

        return movie.ToMovieModel();
    }

    public async Task<MovieModel> UpdateMovie(Id<Movie> id, Validated<MovieRequest> request, CancellationToken token)
    {
        this.logger.LogDebug("Updating the movie with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, id, token);

        var kind = this.FindKind(list, Id.For<MovieKind>(request.Value.KindId));

        movie.Update(request, kind);
        movie.ListItem?.SetProperties(movie);

        list.SortItems();

        await this.dbContext.SaveChangesAsync(token);

        return movie.ToMovieModel();
    }

    public async Task RemoveMovie(Id<Movie> id, CancellationToken token)
    {
        this.logger.LogDebug("Removing the movie with ID: {Id}", id.Value);

        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, id, token);

        if (movie.FranchiseItem is { } item)
        {
            item.ParentFranchise.DetachMovie(movie);
        }

        list.RemoveMovie(movie);
        list.SortItems();

        this.dbContext.ListItems.Remove(movie.ListItem!);
        this.dbContext.Movies.Remove(movie);
        await this.dbContext.SaveChangesAsync(token);
    }

    public async Task<BinaryContent> GetMoviePoster(Id<Movie> movieId, CancellationToken token)
    {
        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, movieId, token);

        var poster = await this.dbContext.MoviePosters
            .Where(poster => poster.Movie == movie)
            .FirstOrDefaultAsync(token)
            ?? throw this.PosterNotFound(movieId);

        return poster.ToPosterModel();
    }

    public Task<PosterHash> SetMoviePoster(
        Id<Movie> movieId,
        StreamableContent content,
        CancellationToken token) =>
        this.SetMoviePoster(movieId, () => Task.FromResult(content), token);

    public Task<PosterHash> SetMoviePoster(
        Id<Movie> movieId,
        Validated<PosterUrlRequest> request,
        CancellationToken token) =>
        this.SetMoviePoster(movieId, () => this.posterProvider.FetchPoster(request, token), token);

    public Task<PosterHash> SetMoviePoster(
        Id<Movie> movieId,
        Validated<PosterImdbMediaRequest> request,
        CancellationToken token) =>
        this.SetMoviePoster(movieId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task RemoveMoviePoster(Id<Movie> movieId, CancellationToken token)
    {
        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, movieId, token);

        var poster = await this.dbContext.MoviePosters
            .Where(poster => poster.Movie == movie)
            .FirstOrDefaultAsync(token);

        if (poster is not null)
        {
            this.dbContext.MoviePosters.Remove(poster);
        }

        movie.PosterHash = null;
        await this.dbContext.SaveChangesAsync(token);
    }

    private async Task<Movie> FindMovie(CineasteList list, Id<Movie> id, CancellationToken token)
    {
        var movie = list.Items
            .Select(item => item.Movie)
            .WhereNotNull()
            .FirstOrDefault(movie => movie.Id == id)
            ?? throw this.NotFound(id);

        await this.dbContext.Entry(movie)
            .Reference(m => m.FranchiseItem)
            .Query()
            .Include(item => item.ParentFranchise)
            .LoadAsync(token);

        if (movie.FranchiseItem is not null)
        {
            await this.LoadFullFranchise(movie.FranchiseItem.ParentFranchise, token);
        }

        return movie;
    }

    private async Task LoadFullFranchise(Franchise franchise, CancellationToken token)
    {
        await this.dbContext.Entry(franchise)
            .Reference(f => f.FranchiseItem)
            .Query()
            .Include(item => item.ParentFranchise)
            .LoadAsync(token);

        if (franchise.FranchiseItem is not null)
        {
            await this.LoadFullFranchise(franchise.FranchiseItem.ParentFranchise, token);
        } else
        {
            await this.LoadChildren(franchise, token);
        }
    }

    private async Task LoadChildren(Franchise franchise, CancellationToken token)
    {
        await this.dbContext.Entry(franchise)
            .Collection(f => f.Children)
            .Query()
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.Periods)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.SpecialEpisodes)
                    .ThenInclude(episode => episode.AllTitles)
            .Include(item => item.Franchise)
                .ThenInclude(f => f!.AllTitles)
            .LoadAsync(token);

        foreach (var childFranchise in franchise.Children.Select(item => item.Franchise).WhereNotNull())
        {
            await this.LoadChildren(childFranchise, token);
        }
    }

    private async Task<CineasteList> FindList(CancellationToken token)
    {
        var list = await this.dbContext.Lists.FirstOrDefaultAsync(token) ?? throw this.ListNotFound();

        await this.dbContext.Entry(list)
            .Reference(list => list.Configuration)
            .LoadAsync(token);

        await this.dbContext.Entry(list)
            .Collection(list => list.MovieKinds)
            .LoadAsync(token);

        await this.dbContext.Entry(list)
            .Collection(list => list.SeriesKinds)
            .LoadAsync(token);

        await this.dbContext.Entry(list)
            .Collection(list => list.Items)
            .Query()
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.AllTitles)
            .Include(item => item.Movie)
                .ThenInclude(movie => movie!.FranchiseItem)
                    .ThenInclude(item => item!.ParentFranchise)
            .Include(item => item.Series)
                .ThenInclude(series => series!.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.FranchiseItem)
                    .ThenInclude(item => item!.ParentFranchise)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.Periods)
            .Include(item => item.Series)
                .ThenInclude(series => series!.Seasons)
                    .ThenInclude(season => season.AllTitles)
            .Include(item => item.Series)
                .ThenInclude(series => series!.SpecialEpisodes)
                    .ThenInclude(episode => episode.AllTitles)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.AllTitles)
            .Include(item => item.Franchise)
                .ThenInclude(franchise => franchise!.FranchiseItem)
                    .ThenInclude(item => item!.ParentFranchise)
            .AsSplitQuery()
            .LoadAsync(token);

        return list;
    }

    private MovieKind FindKind(CineasteList list, Id<MovieKind> id) =>
        list.MovieKinds
            .FirstOrDefault(kind => kind.Id == id)
            ?? throw this.NotFound(id);

    private Franchise FindFranchise(CineasteList list, Id<Franchise> id) =>
        list.Items
            .Select(item => item.Franchise)
            .WhereNotNull()
            .FirstOrDefault(franchise => franchise.Id == id)
            ?? throw this.NotFound(id);

    private Task<PosterHash> SetMoviePoster(
        Id<Movie> movieId,
        Func<Task<StreamableContent>> getContent,
        CancellationToken token) =>
        this.SetMoviePoster(movieId, async () => await (await getContent()).ReadDataAsync(token), token);

    private async Task<PosterHash> SetMoviePoster(
        Id<Movie> movieId,
        Func<Task<BinaryContent>> getContent,
        CancellationToken token)
    {
        var list = await this.FindList(token);
        var movie = await this.FindMovie(list, movieId, token);

        var content = await getContent();

        if (await this.dbContext.MoviePosters.FirstOrDefaultAsync(poster => poster.Movie == movie, token)
            is { } existingPoster)
        {
            this.dbContext.MoviePosters.Remove(existingPoster);
        }

        var poster = new MoviePoster(Id.Create<MoviePoster>(), movie, content.Data, content.Type);

        var hash = PosterHash.ForPoster(content.Data);
        movie.PosterHash = hash;

        this.dbContext.MoviePosters.Add(poster);
        await this.dbContext.SaveChangesAsync(token);

        return hash;
    }

    private NotFoundException ListNotFound() =>
        new(Resources.List, "Could not find the list");

    private CineasteException NotFound(Id<Movie> id) =>
        new NotFoundException(Resources.Movie, $"Could not find a movie with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<MovieKind> id) =>
        new NotFoundException(Resources.MovieKind, $"Could not find a movie kind with ID {id.Value}")
            .WithProperty(id);

    private CineasteException NotFound(Id<Franchise> id) =>
        new NotFoundException(Resources.Franchise, $"Could not find a franchise with ID {id.Value}")
            .WithProperty(id);

    private CineasteException PosterNotFound(Id<Movie> movieId) =>
        new NotFoundException(Resources.Poster, $"Could not find a poster for movie with ID {movieId.Value}")
            .WithProperty(movieId);
}
