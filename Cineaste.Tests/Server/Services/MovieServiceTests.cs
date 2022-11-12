namespace Cineaste.Server.Services;

using System.Globalization;

using Cineaste.Core.Domain;
using Cineaste.Persistence;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.Movie;
using Cineaste.Shared.Models.Shared;
using Cineaste.Shared.Validation;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

public class MovieServiceTests
{
    private readonly CineasteList list;
    private readonly MovieKind kind;

    private readonly CineasteDbContext dbContext;
    private readonly MovieService movieService;

    public MovieServiceTests()
    {
        this.list = this.CreateList();
        this.kind = this.CreateKind(this.list);

        this.dbContext = this.CreateInMemoryDb(this.list, this.kind);
        this.movieService = new MovieService(this.dbContext, new NullLogger<MovieService>());
    }

    [Fact(DisplayName = "GetMovie should return the correct movie")]
    public async Task GetMovieShouldReturnCorrectMovie()
    {
        var movie = this.CreateMovie();
        this.list.AddMovie(movie);

        this.dbContext.Movies.Add(movie);
        this.dbContext.SaveChanges();

        var model = await this.movieService.GetMovie(movie.Id);

        Assert.Equal(movie.Id.Value, model.Id);

        Assert.Equal(
            from title in movie.Titles
            where !title.IsOriginal
            orderby title.Priority
            select title.Name,
            from title in model.Titles
            orderby title.Priority
            select title.Name);

        Assert.Equal(
            from title in movie.Titles
            where title.IsOriginal
            orderby title.Priority
            select title.Name,
            from title in model.OriginalTitles
            orderby title.Priority
            select title.Name);

        Assert.Equal(movie.Year, model.Year);
        Assert.Equal(movie.IsWatched, model.IsWatched);
        Assert.Equal(movie.IsReleased, model.IsReleased);
        Assert.Equal(movie.Kind.Id.Value, model.Kind.Id);
        Assert.Equal(movie.ImdbId, model.ImdbId);
        Assert.Equal(movie.RottenTomatoesId, model.RottenTomatoesId);
    }

    [Fact(DisplayName = "CreateMovie should put it into the database")]
    public async Task CreateMovieShouldPutItIntoDb()
    {
        var request = this.CreateMovieRequest();

        var model = await this.movieService.CreateMovie(request.Validated());

        var movie = this.dbContext.Movies.Find(Id.Create<Movie>(model.Id));

        Assert.NotNull(movie);

        Assert.Equal(
            from title in request.Titles
            orderby title.Priority
            select title.Name,
            from title in movie.Titles
            where !title.IsOriginal
            orderby title.Priority
            select title.Name);

        Assert.Equal(
            from title in request.OriginalTitles
            orderby title.Priority
            select title.Name,
            from title in movie.Titles
            where title.IsOriginal
            orderby title.Priority
            select title.Name);

        Assert.Equal(request.Year, movie.Year);
        Assert.Equal(request.IsWatched, movie.IsWatched);
        Assert.Equal(request.IsReleased, movie.IsReleased);
        Assert.Equal(request.KindId, movie.Kind.Id.Value);
        Assert.Equal(request.ImdbId, movie.ImdbId);
        Assert.Equal(request.RottenTomatoesId, movie.RottenTomatoesId);
    }

    [Fact(DisplayName = "CreateMovie should return a correct model")]
    public async Task CreateMovieShouldReturnCorrectModel()
    {
        var request = this.CreateMovieRequest();

        var model = await this.movieService.CreateMovie(request.Validated());

        Assert.Equal(
            from title in request.Titles
            orderby title.Priority
            select title.Name,
            from title in model.Titles
            orderby title.Priority
            select title.Name);

        Assert.Equal(
            from title in request.OriginalTitles
            orderby title.Priority
            select title.Name,
            from title in model.OriginalTitles
            orderby title.Priority
            select title.Name);

        Assert.Equal(request.Year, model.Year);
        Assert.Equal(request.IsWatched, model.IsWatched);
        Assert.Equal(request.IsReleased, model.IsReleased);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);
    }

    [Fact(DisplayName = "UpdateMovie should update it in the database")]
    public async Task UpdateMovieShouldUpdateItInDb()
    {
        var dbMovie = this.CreateMovie();
        this.list.AddMovie(dbMovie);

        this.dbContext.Movies.Add(dbMovie);
        this.dbContext.SaveChanges();

        var request = this.CreateMovieRequest();

        var model = await this.movieService.UpdateMovie(dbMovie.Id, request.Validated());

        var movie = this.dbContext.Movies.Find(Id.Create<Movie>(model.Id));

        Assert.NotNull(movie);

        Assert.Equal(
            from title in request.Titles
            orderby title.Priority
            select title.Name,
            from title in movie.Titles
            where !title.IsOriginal
            orderby title.Priority
            select title.Name);

        Assert.Equal(
            from title in request.OriginalTitles
            orderby title.Priority
            select title.Name,
            from title in movie.Titles
            where title.IsOriginal
            orderby title.Priority
            select title.Name);

        Assert.Equal(request.Year, movie.Year);
        Assert.Equal(request.IsWatched, movie.IsWatched);
        Assert.Equal(request.IsReleased, movie.IsReleased);
        Assert.Equal(request.KindId, movie.Kind.Id.Value);
        Assert.Equal(request.ImdbId, movie.ImdbId);
        Assert.Equal(request.RottenTomatoesId, movie.RottenTomatoesId);
    }

    [Fact(DisplayName = "UpdateMovie should return a correct model")]
    public async Task UpdateMovieShouldReturnCorrectModel()
    {
        var dbMovie = this.CreateMovie();
        this.list.AddMovie(dbMovie);

        this.dbContext.Movies.Add(dbMovie);
        this.dbContext.SaveChanges();

        var request = this.CreateMovieRequest();

        var model = await this.movieService.UpdateMovie(dbMovie.Id, request.Validated());

        Assert.Equal(
            from title in request.Titles
            orderby title.Priority
            select title.Name,
            from title in model.Titles
            orderby title.Priority
            select title.Name);

        Assert.Equal(
            from title in request.OriginalTitles
            orderby title.Priority
            select title.Name,
            from title in model.OriginalTitles
            orderby title.Priority
            select title.Name);

        Assert.Equal(request.Year, model.Year);
        Assert.Equal(request.IsWatched, model.IsWatched);
        Assert.Equal(request.IsReleased, model.IsReleased);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);
    }

    [Fact(DisplayName = "GetMovie should throw if movie isn't found")]
    public async Task GetMovieShouldThrowIfNotFound()
    {
        var dummyId = Id.CreateNew<Movie>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => this.movieService.GetMovie(dummyId));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "DeleteMovie should remote it from the database")]
    public async Task DeleteMovieShouldRemoveItFromDb()
    {
        var movie = this.CreateMovie();
        this.list.AddMovie(movie);

        this.dbContext.Movies.Add(movie);
        this.dbContext.SaveChanges();

        await this.movieService.DeleteMovie(movie.Id);

        Assert.True(this.dbContext.Movies.All(m => m.Id != movie.Id));
    }

    [Fact(DisplayName = "DeleteMovie should throw if movie isn't found")]
    public async Task DeleteMovieShouldThrowIfNotFound()
    {
        var dummyId = Id.CreateNew<Movie>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => this.movieService.DeleteMovie(dummyId));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private Movie CreateMovie() =>
        new(Id.CreateNew<Movie>(), this.CreateTitles(), 2000, true, true, this.kind);

    private IEnumerable<Title> CreateTitles() =>
        new List<Title> { new("Test", 1, false), new("Original Test", 1, true) };

    private MovieRequest CreateMovieRequest() =>
        new(
            list.Id.Value,
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            1999,
            false,
            true,
            this.kind.Id.Value,
            "tt12345678",
            null);

    private CineasteList CreateList() =>
        new(
            Id.CreateNew<CineasteList>(),
            "Test List",
            "test-list",
            new ListConfiguration(
                Id.CreateNew<ListConfiguration>(),
                CultureInfo.InvariantCulture,
                "Season #",
                "Season #",
                ListSortingConfiguration.CreateDefault()));

    private MovieKind CreateKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new MovieKind(Id.CreateNew<MovieKind>(), "Test Kind", black, black, black);

        list.AddMovieKind(kind);

        return kind;
    }

    private CineasteDbContext CreateInMemoryDb(CineasteList list, MovieKind kind)
    {
        var options = new DbContextOptionsBuilder<CineasteDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(MovieServiceTests))
            .Options;

        var context = new CineasteDbContext(options);
        context.Database.EnsureCreated();

        context.MovieKinds.Add(kind);
        context.Lists.Add(list);

        context.SaveChanges();

        return context;
    }
}
