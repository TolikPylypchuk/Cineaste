using Cineaste.Shared.Models.Series;

namespace Cineaste.Application.Services;

public class LimitedSeriesServiceTests(DbFixture dbFixture, ITestOutputHelper output)
    : TestClassBase(dbFixture)
{
    private readonly ILogger<LimitedSeriesService> logger = XUnitLogger.Create<LimitedSeriesService>(output);

    [Fact(DisplayName = "GetLimitedSeries should return the correct limited series")]
    public async Task GetLimitedSeriesShouldReturnCorrectLimitedSeries()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);

        // Act

        var model = await limitedSeriesService.GetLimitedSeries(
            this.data.ListId, limitedSeries.Id, TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(limitedSeries, model);

        Assert.Equal(limitedSeries.Id.Value, model.Id);
        Assert.Equal(limitedSeries.Period.StartMonth.Value, model.Period.StartMonth);
        Assert.Equal(limitedSeries.Period.StartYear, model.Period.StartYear);
        Assert.Equal(limitedSeries.Period.EndMonth.Value, model.Period.EndMonth);
        Assert.Equal(limitedSeries.Period.EndYear, model.Period.EndYear);
        Assert.Equal(limitedSeries.Period.EpisodeCount, model.Period.EpisodeCount);
        Assert.Equal(limitedSeries.Period.IsSingleDayRelease, model.Period.IsSingleDayRelease);
        Assert.Equal(limitedSeries.WatchStatus, model.WatchStatus);
        Assert.Equal(limitedSeries.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(limitedSeries.Channel, model.Channel);
        Assert.Equal(limitedSeries.Kind.Id.Value, model.Kind.Id);
        Assert.Equal(limitedSeries.ImdbId?.Value, model.ImdbId);
        Assert.Equal(limitedSeries.RottenTomatoesId?.Value, model.RottenTomatoesId);
        Assert.Equal(limitedSeries.RottenTomatoesSubId?.Value, model.RottenTomatoesSubId);
    }

    [Fact(DisplayName = "GetLimitedSeries should throw if limited series isn't found")]
    public async Task GetLimitedSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var dummyId = Id.Create<LimitedSeries>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesNotFoundException>(() =>
            limitedSeriesService.GetLimitedSeries(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.LimitedSeriesId);
    }

    [Fact(DisplayName = "AddLimitedSeries should put it into the database")]
    public async Task AddLimitedSeriesShouldPutItIntoDb()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var request = this.CreateLimitedSeriesRequest();

        // Act

        var model = await limitedSeriesService.AddLimitedSeries(
            this.data.ListId, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        var limitedSeries = dbContext.LimitedSeries.Find(Id.For<LimitedSeries>(model.Id));

        Assert.NotNull(limitedSeries);

        AssertTitles(request, limitedSeries);

        Assert.Equal(request.Period.StartMonth, limitedSeries.Period.StartMonth.Value);
        Assert.Equal(request.Period.StartYear, limitedSeries.Period.StartYear);
        Assert.Equal(request.Period.EndMonth, limitedSeries.Period.EndMonth.Value);
        Assert.Equal(request.Period.EndYear, limitedSeries.Period.EndYear);
        Assert.Equal(request.Period.EpisodeCount, limitedSeries.Period.EpisodeCount);
        Assert.Equal(request.Period.IsSingleDayRelease, limitedSeries.Period.IsSingleDayRelease);
        Assert.Equal(request.WatchStatus, limitedSeries.WatchStatus);
        Assert.Equal(request.ReleaseStatus, limitedSeries.ReleaseStatus);
        Assert.Equal(request.Channel, limitedSeries.Channel);
        Assert.Equal(request.KindId, limitedSeries.Kind.Id.Value);
        Assert.Equal(request.ImdbId, limitedSeries.ImdbId?.Value);
        Assert.Equal(request.RottenTomatoesId, limitedSeries.RottenTomatoesId?.Value);
        Assert.Equal(request.RottenTomatoesSubId, limitedSeries.RottenTomatoesSubId?.Value);
    }

    [Fact(DisplayName = "AddLimitedSeries should return a correct model")]
    public async Task AddLimitedSeriesShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var request = this.CreateLimitedSeriesRequest();

        // Act

        var model = await limitedSeriesService.AddLimitedSeries(
            this.data.ListId, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(request, model);

        Assert.Equal(request.Period.StartMonth, model.Period.StartMonth);
        Assert.Equal(request.Period.StartYear, model.Period.StartYear);
        Assert.Equal(request.Period.EndMonth, model.Period.EndMonth);
        Assert.Equal(request.Period.EndYear, model.Period.EndYear);
        Assert.Equal(request.Period.EpisodeCount, model.Period.EpisodeCount);
        Assert.Equal(request.Period.IsSingleDayRelease, model.Period.IsSingleDayRelease);
        Assert.Equal(request.WatchStatus, model.WatchStatus);
        Assert.Equal(request.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(request.Channel, model.Channel);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);
        Assert.Equal(request.RottenTomatoesSubId, model.RottenTomatoesSubId);
    }

    [Fact(DisplayName = "AddLimitedSeries should throw if the kind is not found")]
    public async Task AddLimitedSeriesShouldThrowIfKindNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var dummyKindId = Id.Create<SeriesKind>();
        var request = this.CreateLimitedSeriesRequest() with { KindId = dummyKindId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesKindNotFoundException>(() =>
            limitedSeriesService.AddLimitedSeries(
                this.data.ListId, request.Validated(), TestContext.Current.CancellationToken));

        Assert.Equal(dummyKindId, exception.KindId);
    }

    [Fact(DisplayName = "UpdateLimitedSeries should update it in the database")]
    public async Task UpdateLimitedSeriesShouldUpdateItInDb()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);

        var request = this.CreateLimitedSeriesRequest();

        // Act

        var model = await limitedSeriesService.UpdateLimitedSeries(
            this.data.ListId, limitedSeries.Id, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        var dbLimitedSeries = dbContext.LimitedSeries.Find(Id.For<LimitedSeries>(model.Id));

        Assert.NotNull(dbLimitedSeries);

        AssertTitles(request, dbLimitedSeries);

        Assert.Equal(request.Period.StartMonth, dbLimitedSeries.Period.StartMonth.Value);
        Assert.Equal(request.Period.StartYear, dbLimitedSeries.Period.StartYear);
        Assert.Equal(request.Period.EndMonth, dbLimitedSeries.Period.EndMonth.Value);
        Assert.Equal(request.Period.EndYear, dbLimitedSeries.Period.EndYear);
        Assert.Equal(request.Period.EpisodeCount, dbLimitedSeries.Period.EpisodeCount);
        Assert.Equal(request.Period.IsSingleDayRelease, dbLimitedSeries.Period.IsSingleDayRelease);
        Assert.Equal(request.WatchStatus, dbLimitedSeries.WatchStatus);
        Assert.Equal(request.ReleaseStatus, dbLimitedSeries.ReleaseStatus);
        Assert.Equal(request.Channel, dbLimitedSeries.Channel);
        Assert.Equal(request.KindId, dbLimitedSeries.Kind.Id.Value);
        Assert.Equal(request.ImdbId, dbLimitedSeries.ImdbId?.Value);
        Assert.Equal(request.RottenTomatoesId, dbLimitedSeries.RottenTomatoesId?.Value);
        Assert.Equal(request.RottenTomatoesSubId, dbLimitedSeries.RottenTomatoesSubId?.Value);
    }

    [Fact(DisplayName = "UpdateLimitedSeries should return a correct model")]
    public async Task UpdateLimitedSeriesShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);

        var request = this.CreateLimitedSeriesRequest();

        // Act

        var model = await limitedSeriesService.UpdateLimitedSeries(
            this.data.ListId, limitedSeries.Id, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(request, model);

        Assert.Equal(request.Period.StartMonth, model.Period.StartMonth);
        Assert.Equal(request.Period.StartYear, model.Period.StartYear);
        Assert.Equal(request.Period.EndMonth, model.Period.EndMonth);
        Assert.Equal(request.Period.EndYear, model.Period.EndYear);
        Assert.Equal(request.Period.EpisodeCount, model.Period.EpisodeCount);
        Assert.Equal(request.Period.IsSingleDayRelease, model.Period.IsSingleDayRelease);
        Assert.Equal(request.WatchStatus, model.WatchStatus);
        Assert.Equal(request.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(request.Channel, model.Channel);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);
        Assert.Equal(request.RottenTomatoesSubId, model.RottenTomatoesSubId);
    }

    [Fact(DisplayName = "UpdateLimitedSeries should throw if not found")]
    public async Task UpdateLimitedSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var request = this.CreateLimitedSeriesRequest();
        var dummyId = Id.Create<LimitedSeries>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesNotFoundException>(() =>
            limitedSeriesService.UpdateLimitedSeries(
                this.data.ListId, dummyId, request.Validated(), TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.LimitedSeriesId);
    }

    [Fact(DisplayName = "RemoveLimitedSeries should remote it from the database")]
    public async Task RemoveLimitedSeriesShouldRemoveItFromDb()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var limitedSeries = await this.data.CreateLimitedSeries(dbContext);

        // Act

        await limitedSeriesService.RemoveLimitedSeries(
            this.data.ListId, limitedSeries.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.True(dbContext.LimitedSeries.All(m => m.Id != limitedSeries.Id));
    }

    [Fact(DisplayName = "RemoveLimitedSeries should throw if limited series isn't found")]
    public async Task RemoveLimitedSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var limitedSeriesService = this.CreateLimitedSeriesService(dbContext);

        var dummyId = Id.Create<LimitedSeries>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<LimitedSeriesNotFoundException>(() =>
            limitedSeriesService.RemoveLimitedSeries(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.LimitedSeriesId);
    }

    private LimitedSeriesService CreateLimitedSeriesService(CineasteDbContext dbContext) =>
        new(dbContext, this.data.PosterProvider, this.data.PosterUrlProvider, this.logger);

    private LimitedSeriesRequest CreateLimitedSeriesRequest() =>
        new(
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            new(Month.January.Value, 2000, Month.February.Value, 2000, 10, false),
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            "Test",
            this.data.SeriesKindId.Value,
            "tt12345678",
            null,
            null,
            null);
}
