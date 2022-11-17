namespace Cineaste.Server.Services;

using System.Globalization;

using Cineaste.Core.Domain;
using Cineaste.Persistence;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.Movie;
using Cineaste.Shared.Models.Shared;
using Cineaste.Shared.Validation;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public class MovieServiceTests : ServiceTestsBase
{
    private readonly CineasteList list;
    private readonly MovieKind kind;
    private readonly ILogger<MovieService> logger;

    public MovieServiceTests()
    {
        this.list = this.CreateList();
        this.kind = this.CreateKind(this.list);
        this.logger = new NullLogger<MovieService>();
    }

    [Fact(DisplayName = "GetMovie should return the correct movie")]
    public async Task GetMovieShouldReturnCorrectMovie()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var movie = this.CreateMovie();
        this.list.AddMovie(movie);

        dbContext.Movies.Add(movie);
        dbContext.SaveChanges();

        var model = await movieService.GetMovie(movie.Id);

        this.AssertTitles(movie, model);

        Assert.Equal(movie.Id.Value, model.Id);
        Assert.Equal(movie.Year, model.Year);
        Assert.Equal(movie.IsWatched, model.IsWatched);
        Assert.Equal(movie.IsReleased, model.IsReleased);
        Assert.Equal(movie.Kind.Id.Value, model.Kind.Id);
        Assert.Equal(movie.ImdbId, model.ImdbId);
        Assert.Equal(movie.RottenTomatoesId, model.RottenTomatoesId);
    }

    [Fact(DisplayName = "GetMovie should throw if movie isn't found")]
    public async Task GetMovieShouldThrowIfNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyId = Id.CreateNew<Movie>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => movieService.GetMovie(dummyId));

        Assert.Equal("NotFound.Movie", exception.MessageCode);
        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateMovie should put it into the database")]
    public async Task CreateMovieShouldPutItIntoDb()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var request = this.CreateMovieRequest();

        var model = await movieService.CreateMovie(request.Validated());

        var movie = dbContext.Movies.Find(Id.Create<Movie>(model.Id));

        Assert.NotNull(movie);

        this.AssertTitles(request, movie);

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
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var request = this.CreateMovieRequest();

        var model = await movieService.CreateMovie(request.Validated());

        this.AssertTitles(request, model);

        Assert.Equal(request.Year, model.Year);
        Assert.Equal(request.IsWatched, model.IsWatched);
        Assert.Equal(request.IsReleased, model.IsReleased);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);
    }

    [Fact(DisplayName = "CreateMovie should throw if the list is not found")]
    public async Task CreateMovieShouldThrowIfListNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyListId = Id.CreateNew<CineasteList>();
        var request = this.CreateMovieRequest() with { ListId = dummyListId.Value };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.CreateMovie(request.Validated()));

        Assert.Equal("NotFound.List", exception.MessageCode);
        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyListId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateMovie should throw if the kind is not found")]
    public async Task CreateMovieShouldThrowIfKindNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyKindId = Id.CreateNew<MovieKind>();
        var request = this.CreateMovieRequest() with { KindId = dummyKindId.Value };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.CreateMovie(request.Validated()));

        Assert.Equal("NotFound.MovieKind", exception.MessageCode);
        Assert.Equal("Resource.MovieKind", exception.Resource);
        Assert.Equal(dummyKindId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateMovie should throw if the kind doesn't belong to list")]
    public async Task CreateMovieShouldThrowIfKindDoesNotBelongToList()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var otherList = this.CreateList();
        var otherKind = this.CreateKind(otherList);

        dbContext.MovieKinds.Add(otherKind);
        dbContext.Lists.Add(otherList);
        dbContext.SaveChanges();

        var request = this.CreateMovieRequest() with { KindId = otherKind.Id.Value };

        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            movieService.CreateMovie(request.Validated()));

        Assert.Equal("BadRequest.MovieKind.WrongList", exception.MessageCode);
        Assert.Equal(this.list.Id, exception.Properties["listId"]);
        Assert.Equal(otherKind.Id, exception.Properties["kindId"]);
    }

    [Fact(DisplayName = "UpdateMovie should update it in the database")]
    public async Task UpdateMovieShouldUpdateItInDb()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var movie = this.CreateMovie();
        this.list.AddMovie(movie);

        dbContext.Movies.Add(movie);
        dbContext.SaveChanges();

        var request = this.CreateMovieRequest();

        var model = await movieService.UpdateMovie(movie.Id, request.Validated());

        var dbMovie = dbContext.Movies.Find(Id.Create<Movie>(model.Id));

        Assert.NotNull(dbMovie);

        this.AssertTitles(request, model);

        Assert.Equal(request.Year, dbMovie.Year);
        Assert.Equal(request.IsWatched, dbMovie.IsWatched);
        Assert.Equal(request.IsReleased, dbMovie.IsReleased);
        Assert.Equal(request.KindId, dbMovie.Kind.Id.Value);
        Assert.Equal(request.ImdbId, dbMovie.ImdbId);
        Assert.Equal(request.RottenTomatoesId, dbMovie.RottenTomatoesId);
    }

    [Fact(DisplayName = "UpdateMovie should return a correct model")]
    public async Task UpdateMovieShouldReturnCorrectModel()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var movie = this.CreateMovie();
        this.list.AddMovie(movie);

        dbContext.Movies.Add(movie);
        dbContext.SaveChanges();

        var request = this.CreateMovieRequest();

        var model = await movieService.UpdateMovie(movie.Id, request.Validated());

        this.AssertTitles(request, model);

        Assert.Equal(request.Year, model.Year);
        Assert.Equal(request.IsWatched, model.IsWatched);
        Assert.Equal(request.IsReleased, model.IsReleased);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);
    }

    [Fact(DisplayName = "UpdateMovie should throw if not found")]
    public async Task UpdateMovieShouldThrowIfNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var movie = this.CreateMovie();
        this.list.AddMovie(movie);

        dbContext.Movies.Add(movie);
        dbContext.SaveChanges();

        var request = this.CreateMovieRequest();
        var dummyId = Id.CreateNew<Movie>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.UpdateMovie(dummyId, request.Validated()));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "UpdateMovie should throw if movie doesn't belong to list")]
    public async Task UpdateMovieShouldThrowIfMovieDoesNotBelongToList()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var movie = this.CreateMovie();
        this.list.AddMovie(movie);

        dbContext.Movies.Add(movie);

        var otherList = this.CreateList();
        dbContext.Lists.Add(otherList);

        dbContext.SaveChanges();

        var request = this.CreateMovieRequest() with { ListId = otherList.Id.Value };

        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            movieService.UpdateMovie(movie.Id, request.Validated()));

        Assert.Equal("BadRequest.Movie.WrongList", exception.MessageCode);
        Assert.Equal(movie.Id, exception.Properties["movieId"]);
        Assert.Equal(otherList.Id, exception.Properties["listId"]);
    }

    [Fact(DisplayName = "DeleteMovie should remote it from the database")]
    public async Task DeleteMovieShouldRemoveItFromDb()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var movie = this.CreateMovie();
        this.list.AddMovie(movie);

        dbContext.Movies.Add(movie);
        dbContext.SaveChanges();

        await movieService.DeleteMovie(movie.Id);

        Assert.True(dbContext.Movies.All(m => m.Id != movie.Id));
    }

    [Fact(DisplayName = "DeleteMovie should throw if movie isn't found")]
    public async Task DeleteMovieShouldThrowIfNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new MovieService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyId = Id.CreateNew<Movie>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => movieService.DeleteMovie(dummyId));

        Assert.Equal("NotFound.Movie", exception.MessageCode);
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

    private void SetUpDb(CineasteDbContext context)
    {
        context.MovieKinds.Add(this.kind);
        context.Lists.Add(this.list);

        context.SaveChanges();
    }
}
