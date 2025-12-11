using System.Security.Cryptography;

namespace Cineaste.Application.Services;

public sealed class MovieServicePosterTests(DbFixture dbFixture, ITestOutputHelper output)
    : TestClassBase(dbFixture)
{
    private readonly ILogger<MovieService> logger = XUnitLogger.Create<MovieService>(output);

    [Fact(DisplayName = "GetMoviePoster should get the movie poster")]
    public async Task GetMoviePoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var movie = await this.data.CreateMovie(dbContext);
        var poster = await this.data.CreateMoviePoster(movie, dbContext);

        // Act

        var posterContent = await movieService.GetMoviePoster(
            this.data.ListId, movie.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetMoviePoster should throw if movie isn't found")]
    public async Task GetMoviePosterMovieNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<MovieNotFoundException>(() =>
            movieService.GetMoviePoster(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.MovieId);
    }

    [Fact(DisplayName = "GetMoviePoster should throw if poster isn't found")]
    public async Task GetMoviePosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var movie = await this.data.CreateMovie(dbContext);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<MoviePosterNotFoundException>(() =>
            movieService.GetMoviePoster(this.data.ListId, movie.Id, TestContext.Current.CancellationToken));

        Assert.Equal(movie.Id, exception.MovieId);
    }

    [Fact(DisplayName = "SetMoviePoster should set the movie poster from a stream")]
    public async Task SetMoviePosterStream()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var movie = await this.data.CreateMovie(dbContext);
        var poster = await this.data.CreateMoviePoster(movie, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await movieService.SetMoviePoster(
            this.data.ListId, movie.Id, posterData, TestContext.Current.CancellationToken);

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

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var dummyId = Id.Create<Movie>();
        var posterData = this.data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<MovieNotFoundException>(() =>
            movieService.SetMoviePoster(this.data.ListId, dummyId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.MovieId);
    }

    [Fact(DisplayName = "SetMoviePoster should set the movie poster from a URL")]
    public async Task SetMoviePosterUrl()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var movie = await this.data.CreateMovie(dbContext);
        var poster = await this.data.CreateMoviePoster(movie, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterUrlRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await movieService.SetMoviePoster(
            this.data.ListId, movie.Id, request, TestContext.Current.CancellationToken);

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

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var dummyId = Id.Create<Movie>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<MovieNotFoundException>(() =>
            movieService.SetMoviePoster(this.data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.MovieId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetMoviePoster should set the movie poster from IMDb media")]
    public async Task SetMoviePosterImdbMovieMedia()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var movie = await this.data.CreateMovie(dbContext);
        var poster = await this.data.CreateMoviePoster(movie, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterImdbMediaRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await movieService.SetMoviePoster(
            this.data.ListId, movie.Id, request, TestContext.Current.CancellationToken);

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

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var dummyId = Id.Create<Movie>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<MovieNotFoundException>(() =>
            movieService.SetMoviePoster(this.data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.MovieId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "RemoveMoviePoster should remove the movie poster")]
    public async Task RemoveMoviePoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var movie = await this.data.CreateMovie(dbContext);
        var poster = await this.data.CreateMoviePoster(movie, dbContext);

        // Act

        await movieService.RemoveMoviePoster(this.data.ListId, movie.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.MoviePosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveMoviePoster should not throw if poster isn't found")]
    public async Task RemoveMoviePosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var movie = await this.data.CreateMovie(dbContext);

        // Act + Assert

        var exception = await Record.ExceptionAsync(() =>
            movieService.RemoveMoviePoster(this.data.ListId, movie.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveMoviePoster should throw if the movie isn't found")]
    public async Task RemoveMoviePosterMovieNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var movieService = this.CreateMovieService(dbContext);

        var dummyId = Id.Create<Movie>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<MovieNotFoundException>(() =>
            movieService.RemoveMoviePoster(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.MovieId);
    }

    private MovieService CreateMovieService(CineasteDbContext dbContext) =>
        new(dbContext, this.data.PosterProvider, this.data.PosterUrlProvider, this.logger);
}
