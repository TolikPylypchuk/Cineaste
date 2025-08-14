using System.Security.Cryptography;

namespace Cineaste.Services;

public sealed class MovieServicePosterTests(DataFixture data, ITestOutputHelper output)
{
    private readonly ILogger<MovieService> logger = XUnitLogger.Create<MovieService>(output);

    [Fact(DisplayName = "GetMoviePoster should get the movie poster")]
    public async Task GetMoviePoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var poster = await data.CreateMoviePoster(movie, dbContext);

        // Act

        var posterContent = await movieService.GetMoviePoster(movie.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetMoviePoster should throw if movie isn't found")]
    public async Task GetMoviePosterMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.GetMoviePoster(dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "GetMoviePoster should throw if poster isn't found")]
    public async Task GetMoviePosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.GetMoviePoster(movie.Id, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Poster", exception.Resource);
        Assert.Equal(movie.Id, exception.Properties["movieId"]);
    }

    [Fact(DisplayName = "SetMoviePoster should set the movie poster from a stream")]
    public async Task SetMoviePosterStream()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var poster = await data.CreateMoviePoster(movie, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await movieService.SetMoviePoster(
            movie.Id, posterData, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.MoviePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.MoviePosters.FirstOrDefault(p => p.Movie == movie);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetMoviePoster from stream should throw if the movie isn't found")]
    public async Task SetMoviePosterStreamMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Movie>();
        var posterData = data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.SetMoviePoster(dummyId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "SetMoviePoster should set the movie poster from a URL")]
    public async Task SetMoviePosterUrl()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var poster = await data.CreateMoviePoster(movie, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterUrlRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await movieService.SetMoviePoster(movie.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.MoviePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.MoviePosters.FirstOrDefault(p => p.Movie == movie);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetMoviePoster from a URL should throw if the movie isn't found")]
    public async Task SetMoviePosterUrlMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Movie>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.SetMoviePoster(dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetMoviePoster should set the movie poster from IMDb media")]
    public async Task SetMoviePosterImdbMovieMedia()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var poster = await data.CreateMoviePoster(movie, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterImdbMediaRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await movieService.SetMoviePoster(movie.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.MoviePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.MoviePosters.FirstOrDefault(p => p.Movie == movie);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetMoviePoster from IMDb media should throw if the movie isn't found")]
    public async Task SetMoviePosterImdbMediaMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Movie>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.SetMoviePoster(dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "RemoveMoviePoster should remove the movie poster")]
    public async Task RemoveMoviePoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var poster = await data.CreateMoviePoster(movie, dbContext);

        // Act

        await movieService.RemoveMoviePoster(movie.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.MoviePosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveMoviePoster should not throw if poster isn't found")]
    public async Task RemoveMoviePosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var movie = await data.CreateMovie(dbContext);

        // Act + Assert

        var exception = await Record.ExceptionAsync(() =>
            movieService.RemoveMoviePoster(movie.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveMoviePoster should throw if the movie isn't found")]
    public async Task RemoveMoviePosterMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var movieService = new MovieService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.RemoveMoviePoster(dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Movie", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }
}
