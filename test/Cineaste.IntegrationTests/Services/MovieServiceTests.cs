using Cineaste.Shared.Models.Movie;

namespace Cineaste.Services;

public class MovieServiceTests(DataFixture data, ITestOutputHelper output)
{
    private readonly ILogger<MovieService> logger = XUnitLogger.Create<MovieService>(output);

    [Fact(DisplayName = "GetMovie should return the correct movie")]
    public async Task GetMovieShouldReturnCorrectMovie()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);

        // Act

        var model = await movieService.GetMovie(data.ListId, movie.Id, TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(movie, model);

        Assert.Equal(movie.Id.Value, model.Id);
        Assert.Equal(movie.Year, model.Year);
        Assert.Equal(movie.IsWatched, model.IsWatched);
        Assert.Equal(movie.IsReleased, model.IsReleased);
        Assert.Equal(movie.Kind.Id.Value, model.Kind.Id);
        Assert.Equal(movie.ImdbId?.Value, model.ImdbId);
        Assert.Equal(movie.RottenTomatoesId?.Value, model.RottenTomatoesId);
    }

    [Fact(DisplayName = "GetMovie should throw if movie isn't found")]
    public async Task GetMovieShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.GetMovie(data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("NotFound.Movie", exception.MessageCode);
        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "AddMovie should put it into the database")]
    public async Task AddMovieShouldPutItIntoDb()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var request = this.CreateMovieRequest();

        // Act

        var model = await movieService.AddMovie(
            data.ListId, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        var movie = dbContext.Movies.Find(Id.For<Movie>(model.Id));

        Assert.NotNull(movie);

        AssertTitles(request, movie);

        Assert.Equal(request.Year, movie.Year);
        Assert.Equal(request.IsWatched, movie.IsWatched);
        Assert.Equal(request.IsReleased, movie.IsReleased);
        Assert.Equal(request.KindId, movie.Kind.Id.Value);
        Assert.Equal(request.ImdbId, movie.ImdbId?.Value);
        Assert.Equal(request.RottenTomatoesId, movie.RottenTomatoesId?.Value);
    }

    [Fact(DisplayName = "AddMovie should return a correct model")]
    public async Task AddMovieShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var request = this.CreateMovieRequest();

        // Act

        var model = await movieService.AddMovie(
            data.ListId, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(request, model);

        Assert.Equal(request.Year, model.Year);
        Assert.Equal(request.IsWatched, model.IsWatched);
        Assert.Equal(request.IsReleased, model.IsReleased);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);
    }

    [Fact(DisplayName = "AddMovie should throw if the kind is not found")]
    public async Task AddMovieShouldThrowIfKindNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var dummyKindId = Id.Create<MovieKind>();
        var request = this.CreateMovieRequest() with { KindId = dummyKindId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.AddMovie(data.ListId, request.Validated(), TestContext.Current.CancellationToken));

        Assert.Equal("NotFound.MovieKind", exception.MessageCode);
        Assert.Equal("Resource.MovieKind", exception.Resource);
        Assert.Equal(dummyKindId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "UpdateMovie should update it in the database")]
    public async Task UpdateMovieShouldUpdateItInDb()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);

        var request = this.CreateMovieRequest();

        // Act

        var model = await movieService.UpdateMovie(
            data.ListId, movie.Id, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        var dbMovie = dbContext.Movies.Find(Id.For<Movie>(model.Id));

        Assert.NotNull(dbMovie);

        AssertTitles(request, model);

        Assert.Equal(request.Year, dbMovie.Year);
        Assert.Equal(request.IsWatched, dbMovie.IsWatched);
        Assert.Equal(request.IsReleased, dbMovie.IsReleased);
        Assert.Equal(request.KindId, dbMovie.Kind.Id.Value);
        Assert.Equal(request.ImdbId, dbMovie.ImdbId?.Value);
        Assert.Equal(request.RottenTomatoesId, dbMovie.RottenTomatoesId?.Value);
    }

    [Fact(DisplayName = "UpdateMovie should return a correct model")]
    public async Task UpdateMovieShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);

        var request = this.CreateMovieRequest();

        // Act

        var model = await movieService.UpdateMovie(
            data.ListId, movie.Id, request.Validated(), TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);

        var request = this.CreateMovieRequest();
        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.UpdateMovie(data.ListId, dummyId, request.Validated(), TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "RemoveMovie should remote it from the database")]
    public async Task RemoveMovieShouldRemoveItFromDb()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);

        // Act

        await movieService.RemoveMovie(data.ListId, movie.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.True(dbContext.Movies.All(m => m.Id != movie.Id));
    }

    [Fact(DisplayName = "RemoveMovie should throw if movie isn't found")]
    public async Task RemoveMovieShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.RemoveMovie(data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("NotFound.Movie", exception.MessageCode);
        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private MovieRequest CreateMovieRequest() =>
        new(
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            1999,
            false,
            true,
            data.MovieKindId.Value,
            "tt12345678",
            null,
            null);
}
