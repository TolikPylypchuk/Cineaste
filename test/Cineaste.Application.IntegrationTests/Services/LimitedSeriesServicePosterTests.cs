using System.Security.Cryptography;

namespace Cineaste.Application.Services;

public sealed class LimitedSeriesServicePosterTests(DbFixture dbFixture, ITestOutputHelper output)
    : TestClassBase(dbFixture)
{
    private readonly ILogger<LimitedSeriesService> logger = XUnitLogger.Create<LimitedSeriesService>(output);

    [Fact(DisplayName = "GetLimitedSeriesPoster should get the limited series poster")]
    public async Task GetLimitedSeriesPoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);
        var poster = await this.data.CreateLimitedSeriesPoster(limitedSeries, dbContext);

        // Act

        var posterContent = await limitedSeriesService.GetLimitedSeriesPoster(
            this.data.ListId, limitedSeries.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetLimitedSeriesPoster should throw if limited series isn't found")]
    public async Task GetLimitedSeriesPosterLimitedSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var dummyId = Id.Create<LimitedSeries>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesNotFoundException>(() =>
            limitedSeriesService.GetLimitedSeriesPoster(
                this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.LimitedSeriesId);
    }

    [Fact(DisplayName = "GetLimitedSeriesPoster should throw if poster isn't found")]
    public async Task GetLimitedSeriesPosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesPosterNotFoundException>(() =>
            limitedSeriesService.GetLimitedSeriesPoster(
                this.data.ListId, limitedSeries.Id, TestContext.Current.CancellationToken));

        Assert.Equal(limitedSeries.Id, exception.LimitedSeriesId);
    }

    [Fact(DisplayName = "SetLimitedSeriesPoster should set the limited series poster from a stream")]
    public async Task SetLimitedSeriesPosterStream()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);
        var poster = await this.data.CreateLimitedSeriesPoster(limitedSeries, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await limitedSeriesService.SetLimitedSeriesPoster(
            this.data.ListId, limitedSeries.Id, posterData, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.LimitedSeriesPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.LimitedSeriesPosters.FirstOrDefault(p => p.LimitedSeries == limitedSeries);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetLimitedSeriesPoster from stream should throw if the limited series isn't found")]
    public async Task SetLimitedSeriesPosterStreamLimitedSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var dummyId = Id.Create<LimitedSeries>();
        var posterData = this.data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesNotFoundException>(() =>
            limitedSeriesService.SetLimitedSeriesPoster(
                this.data.ListId, dummyId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.LimitedSeriesId);
    }

    [Fact(DisplayName = "SetLimitedSeriesPoster should set the limited series poster from a URL")]
    public async Task SetLimitedSeriesPosterUrl()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);
        var poster = await this.data.CreateLimitedSeriesPoster(limitedSeries, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterUrlRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await limitedSeriesService.SetLimitedSeriesPoster(
            this.data.ListId, limitedSeries.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.LimitedSeriesPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.LimitedSeriesPosters.FirstOrDefault(p => p.LimitedSeries == limitedSeries);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetLimitedSeriesPoster from a URL should throw if the limited series isn't found")]
    public async Task SetLimitedSeriesPosterUrlLimitedSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var dummyId = Id.Create<LimitedSeries>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesNotFoundException>(() =>
            limitedSeriesService.SetLimitedSeriesPoster(
                this.data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.LimitedSeriesId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetLimitedSeriesPoster should set the limited series poster from IMDb media")]
    public async Task SetLimitedSeriesPosterImdbLimitedSeriesMedia()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);
        var poster = await this.data.CreateLimitedSeriesPoster(limitedSeries, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterImdbMediaRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await limitedSeriesService.SetLimitedSeriesPoster(
            this.data.ListId, limitedSeries.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.LimitedSeriesPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.LimitedSeriesPosters.FirstOrDefault(p => p.LimitedSeries == limitedSeries);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetLimitedSeriesPoster from IMDb media should throw if the limited series isn't found")]
    public async Task SetLimitedSeriesPosterImdbMediaLimitedSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var dummyId = Id.Create<LimitedSeries>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesNotFoundException>(() =>
            limitedSeriesService.SetLimitedSeriesPoster(
                this.data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.LimitedSeriesId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "RemoveLimitedSeriesPoster should remove the limited series poster")]
    public async Task RemoveLimitedSeriesPoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);
        var poster = await this.data.CreateLimitedSeriesPoster(limitedSeries, dbContext);

        // Act

        await limitedSeriesService.RemoveLimitedSeriesPoster(
            this.data.ListId, limitedSeries.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.LimitedSeriesPosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveLimitedSeriesPoster should not throw if poster isn't found")]
    public async Task RemoveLimitedSeriesPosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);

        // Act + Assert

        var exception = await Record.ExceptionAsync(() =>
            limitedSeriesService.RemoveLimitedSeriesPoster(
                this.data.ListId, limitedSeries.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveLimitedSeriesPoster should throw if the limited series isn't found")]
    public async Task RemoveLimitedSeriesPosterLimitedSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var dummyId = Id.Create<LimitedSeries>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesNotFoundException>(() =>
            limitedSeriesService.RemoveLimitedSeriesPoster(
                this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.LimitedSeriesId);
    }

    private LimitedSeriesService CreateLimitedSeriesService(CineasteDbContext dbContext) =>
        new(dbContext, this.data.PosterProvider, this.data.PosterUrlProvider, this.logger);
}
