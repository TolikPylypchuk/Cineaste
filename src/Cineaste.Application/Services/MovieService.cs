namespace Cineaste.Application.Services;

public sealed partial class MovieService(
    CineasteDbContext dbContext,
    IPosterProvider posterProvider,
    ILogger<MovieService> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly IPosterProvider posterProvider = posterProvider;
    private readonly ILogger<MovieService> logger = logger;

    public async Task<MovieModel> GetMovie(Id<CineasteList> listId, Id<Movie> id, CancellationToken token)
    {
        this.LogGetMovie(id, listId);

        var list = await this.FindList(listId, token);
        var movie = await this.FindMovie(list, id, token);

        return movie.ToMovieModel();
    }

    public async Task<MovieModel> AddMovie(
        Id<CineasteList> listId,
        Validated<MovieRequest> request,
        CancellationToken token)
    {
        this.LogAddMovie(listId);

        var list = await this.FindList(listId, token);
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

    public async Task<MovieModel> UpdateMovie(
        Id<CineasteList> listId,
        Id<Movie> id,
        Validated<MovieRequest> request,
        CancellationToken token)
    {
        this.LogUpdateMovie(id, listId);

        var list = await this.FindList(listId, token);
        var movie = await this.FindMovie(list, id, token);

        var kind = this.FindKind(list, Id.For<MovieKind>(request.Value.KindId));

        movie.Update(request, kind);
        movie.ListItem?.SetProperties(movie);

        list.SortItems();

        await this.dbContext.SaveChangesAsync(token);

        return movie.ToMovieModel();
    }

    public async Task RemoveMovie(Id<CineasteList> listId, Id<Movie> id, CancellationToken token)
    {
        this.LogRemoveMovie(id, listId);

        var list = await this.FindList(listId, token);
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

    public async Task<BinaryContent> GetMoviePoster(Id<CineasteList> listId, Id<Movie> movieId, CancellationToken token)
    {
        this.LogGetMoviePoster(movieId, listId);

        var list = await this.FindList(listId, token);
        var movie = await this.FindMovie(list, movieId, token);

        var poster = await this.dbContext.MoviePosters
            .Where(poster => poster.Movie == movie)
            .FirstOrDefaultAsync(token)
            ?? throw new MoviePosterNotFoundException(movieId);

        return poster.ToPosterModel();
    }

    public Task<PosterHash> SetMoviePoster(
        Id<CineasteList> listId,
        Id<Movie> movieId,
        StreamableContent content,
        CancellationToken token) =>
        this.SetMoviePoster(listId, movieId, () => Task.FromResult(content), token);

    public Task<PosterHash> SetMoviePoster(
        Id<CineasteList> listId,
        Id<Movie> movieId,
        Validated<PosterUrlRequest> request,
        CancellationToken token) =>
        this.SetMoviePoster(listId, movieId, () => this.posterProvider.FetchPoster(request, token), token);

    public Task<PosterHash> SetMoviePoster(
        Id<CineasteList> listId,
        Id<Movie> movieId,
        Validated<PosterImdbMediaRequest> request,
        CancellationToken token) =>
        this.SetMoviePoster(listId, movieId, () => this.posterProvider.FetchPoster(request, token), token);

    public async Task RemoveMoviePoster(Id<CineasteList> listId, Id<Movie> movieId, CancellationToken token)
    {
        this.LogRemoveMoviePoster(movieId, listId);

        var list = await this.FindList(listId, token);
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
            ?? throw new MovieNotFoundException(id);

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

    private async Task<CineasteList> FindList(Id<CineasteList> listId, CancellationToken token)
    {
        var list = await this.dbContext.Lists.SingleOrDefaultAsync(list => list.Id == listId, token)
            ?? throw new ListNotFoundException(listId);

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
            ?? throw new MovieKindNotFoundException(id);

    private Franchise FindFranchise(CineasteList list, Id<Franchise> id) =>
        list.Items
            .Select(item => item.Franchise)
            .WhereNotNull()
            .FirstOrDefault(franchise => franchise.Id == id)
            ?? throw new FranchiseNotFoundException(id);

    private Task<PosterHash> SetMoviePoster(
        Id<CineasteList> listId,
        Id<Movie> movieId,
        Func<Task<StreamableContent>> getContent,
        CancellationToken token) =>
        this.SetMoviePoster(listId, movieId, async () => await (await getContent()).ReadDataAsync(token), token);

    private async Task<PosterHash> SetMoviePoster(
        Id<CineasteList> listId,
        Id<Movie> movieId,
        Func<Task<BinaryContent>> getContent,
        CancellationToken token)
    {
        this.LogSetMoviePoster(movieId, listId);

        var list = await this.FindList(listId, token);
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

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting movie {MovieId} in list {ListId}")]
    private partial void LogGetMovie(Id<Movie> movieId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Adding movie to list {ListId}")]
    private partial void LogAddMovie(Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating movie {MovieId} in list {ListId}")]
    private partial void LogUpdateMovie(Id<Movie> movieId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Removing movie {MovieId} from list {ListId}")]
    private partial void LogRemoveMovie(Id<Movie> movieId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Getting the poster for movie {MovieId} in list {ListId}")]
    private partial void LogGetMoviePoster(Id<Movie> movieId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Setting the poster for movie {MovieId} in list {ListId}")]
    private partial void LogSetMoviePoster(Id<Movie> movieId, Id<CineasteList> listId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Removing the poster for movie {MovieId} in list {ListId}")]
    private partial void LogRemoveMoviePoster(Id<Movie> movieId, Id<CineasteList> listId);
}
