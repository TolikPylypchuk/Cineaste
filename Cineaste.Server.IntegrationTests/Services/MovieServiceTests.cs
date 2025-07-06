using Cineaste.Shared.Models.Movie;

namespace Cineaste.Server.Services;

public class MovieServiceTests(ITestOutputHelper output) : ServiceTestsBase
{
    private readonly ILogger<MovieService> logger = XUnitLogger.Create<MovieService>(output);

    [Fact(DisplayName = "GetMovie should return the correct movie")]
    public async Task GetMovieShouldReturnCorrectMovie()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var movie = this.CreateMovie(this.List);

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act

        var model = await movieService.GetMovie(movie.Id);

        // Assert

        AssertTitles(movie, model);

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
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => movieService.GetMovie(dummyId));

        Assert.Equal("NotFound.Movie", exception.MessageCode);
        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateMovie should put it into the database")]
    public async Task CreateMovieShouldPutItIntoDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var request = this.CreateMovieRequest();

        // Act

        var model = await movieService.CreateMovie(request.Validated());

        // Assert

        var movie = dbContext.Movies.Find(Id.For<Movie>(model.Id));

        Assert.NotNull(movie);

        AssertTitles(request, movie);

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
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var request = this.CreateMovieRequest();

        // Act

        var model = await movieService.CreateMovie(request.Validated());

        // Assert

        AssertTitles(request, model);

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
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var dummyListId = Id.Create<CineasteList>();
        var request = this.CreateMovieRequest() with { ListId = dummyListId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.CreateMovie(request.Validated()));

        Assert.Equal("NotFound.List", exception.MessageCode);
        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyListId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateMovie should throw if the kind is not found")]
    public async Task CreateMovieShouldThrowIfKindNotFound()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var dummyKindId = Id.Create<MovieKind>();
        var request = this.CreateMovieRequest() with { KindId = dummyKindId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.CreateMovie(request.Validated()));

        Assert.Equal("NotFound.MovieKind", exception.MessageCode);
        Assert.Equal("Resource.MovieKind", exception.Resource);
        Assert.Equal(dummyKindId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateMovie should throw if the kind doesn't belong to list")]
    public async Task CreateMovieShouldThrowIfKindDoesNotBelongToList()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var otherList = this.CreateList();
        var otherKind = this.CreateMovieKind(otherList);

        dbContext.MovieKinds.Add(otherKind);
        dbContext.Lists.Add(otherList);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateMovieRequest() with { KindId = otherKind.Id.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<InvalidInputException>(() =>
            movieService.CreateMovie(request.Validated()));

        Assert.Equal("BadRequest.MovieKind.WrongList", exception.MessageCode);
        Assert.Equal(this.List.Id, exception.Properties["listId"]);
        Assert.Equal(otherKind.Id, exception.Properties["kindId"]);
    }

    [Fact(DisplayName = "UpdateMovie should update it in the database")]
    public async Task UpdateMovieShouldUpdateItInDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var movie = this.CreateMovie(this.List);

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateMovieRequest();

        // Act

        var model = await movieService.UpdateMovie(movie.Id, request.Validated());

        // Assert

        var dbMovie = dbContext.Movies.Find(Id.For<Movie>(model.Id));

        Assert.NotNull(dbMovie);

        AssertTitles(request, model);

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
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var movie = this.CreateMovie(this.List);

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateMovieRequest();

        // Act

        var model = await movieService.UpdateMovie(movie.Id, request.Validated());

        // Assert

        AssertTitles(request, model);

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
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var movie = this.CreateMovie(this.List);

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateMovieRequest();
        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.UpdateMovie(dummyId, request.Validated()));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "UpdateMovie should throw if movie doesn't belong to list")]
    public async Task UpdateMovieShouldThrowIfMovieDoesNotBelongToList()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var movie = this.CreateMovie(this.List);

        dbContext.Movies.Add(movie);

        var otherList = this.CreateList();
        dbContext.Lists.Add(otherList);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateMovieRequest() with { ListId = otherList.Id.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<InvalidInputException>(() =>
            movieService.UpdateMovie(movie.Id, request.Validated()));

        Assert.Equal("BadRequest.Movie.WrongList", exception.MessageCode);
        Assert.Equal(movie.Id, exception.Properties["movieId"]);
        Assert.Equal(otherList.Id, exception.Properties["listId"]);
    }

    [Fact(DisplayName = "DeleteMovie should remote it from the database")]
    public async Task DeleteMovieShouldRemoveItFromDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var movie = this.CreateMovie(this.List);

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act

        await movieService.DeleteMovie(movie.Id);

        // Assert

        Assert.True(dbContext.Movies.All(m => m.Id != movie.Id));
    }

    [Fact(DisplayName = "DeleteMovie should throw if movie isn't found")]
    public async Task DeleteMovieShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var movieService = new MovieService(dbContext, this.logger);

        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => movieService.DeleteMovie(dummyId));

        Assert.Equal("NotFound.Movie", exception.MessageCode);
        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private MovieRequest CreateMovieRequest() =>
        new(
            this.List.Id.Value,
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            1999,
            false,
            true,
            this.MovieKind.Id.Value,
            "tt12345678",
            null);
}
