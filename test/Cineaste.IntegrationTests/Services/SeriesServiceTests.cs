using Cineaste.Shared.Models.Series;

namespace Cineaste.Services;

public class SeriesServiceTests(DataFixture data, ITestOutputHelper output)
{
    private readonly ILogger<SeriesService> logger = XUnitLogger.Create<SeriesService>(output);

    [Fact(DisplayName = "GetSeries should return the correct series")]
    public async Task GetSeriesShouldReturnCorrectSeries()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var series = await data.CreateSeries(dbContext);

        // Act

        var model = await seriesService.GetSeries(series.Id, TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(series, model);

        Assert.Equal(series.Id.Value, model.Id);
        Assert.Equal(series.WatchStatus, model.WatchStatus);
        Assert.Equal(series.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(series.Kind.Id.Value, model.Kind.Id);
        Assert.Equal(series.ImdbId, model.ImdbId);
        Assert.Equal(series.RottenTomatoesId, model.RottenTomatoesId);

        foreach (var (season, seasonModel) in series.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(model.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            AssertTitles(season, seasonModel);

            Assert.Equal(season.SequenceNumber, seasonModel.SequenceNumber);
            Assert.Equal(season.WatchStatus, seasonModel.WatchStatus);
            Assert.Equal(season.ReleaseStatus, seasonModel.ReleaseStatus);
            Assert.Equal(season.Channel, seasonModel.Channel);

            foreach (var (period, periodModel) in season.Periods.OrderBy(p => p.StartYear).ThenBy(p => p.StartMonth)
                .Zip(seasonModel.Periods.OrderBy(p => p.StartYear).ThenBy(p => p.StartMonth)))
            {
                Assert.Equal(period.StartMonth, periodModel.StartMonth);
                Assert.Equal(period.StartYear, periodModel.StartYear);
                Assert.Equal(period.EndMonth, periodModel.EndMonth);
                Assert.Equal(period.EndYear, periodModel.EndYear);
                Assert.Equal(period.EpisodeCount, periodModel.EpisodeCount);
                Assert.Equal(period.IsSingleDayRelease, periodModel.IsSingleDayRelease);
                Assert.Equal(period.RottenTomatoesId, periodModel.RottenTomatoesId);
            }
        }

        foreach (var (episode, episodeModel) in series.SpecialEpisodes.OrderBy(e => e.SequenceNumber)
            .Zip(model.SpecialEpisodes.OrderBy(e => e.SequenceNumber)))
        {
            AssertTitles(episode, episodeModel);

            Assert.Equal(episode.SequenceNumber, episodeModel.SequenceNumber);
            Assert.Equal(episode.IsWatched, episodeModel.IsWatched);
            Assert.Equal(episode.IsReleased, episodeModel.IsReleased);
            Assert.Equal(episode.Channel, episodeModel.Channel);
            Assert.Equal(episode.Month, episodeModel.Month);
            Assert.Equal(episode.Year, episodeModel.Year);
            Assert.Equal(episode.RottenTomatoesId, episodeModel.RottenTomatoesId);
        }
    }

    [Fact(DisplayName = "GetSeries should throw if series isn't found")]
    public async Task GetSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.GetSeries(dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "AddSeries should put it into the database")]
    public async Task AddSeriesShouldPutItIntoDb()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.AddSeries(request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        var series = dbContext.Series.Find(Id.For<Series>(model.Id));

        Assert.NotNull(series);

        AssertTitles(request, series);

        Assert.Equal(request.WatchStatus, series.WatchStatus);
        Assert.Equal(request.ReleaseStatus, series.ReleaseStatus);
        Assert.Equal(request.KindId, series.Kind.Id.Value);
        Assert.Equal(request.ImdbId, series.ImdbId);
        Assert.Equal(request.RottenTomatoesId, series.RottenTomatoesId);

        foreach (var (seasonRequest, season) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(series.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            AssertTitles(seasonRequest, season);

            Assert.Equal(seasonRequest.SequenceNumber, season.SequenceNumber);
            Assert.Equal(seasonRequest.WatchStatus, season.WatchStatus);
            Assert.Equal(seasonRequest.ReleaseStatus, season.ReleaseStatus);
            Assert.Equal(seasonRequest.Channel, season.Channel);

            foreach (var (periodRequest, period) in seasonRequest.Periods
                .OrderBy(p => p.StartYear)
                .ThenBy(p => p.StartMonth)
                .Zip(season.Periods.OrderBy(p => p.StartYear).ThenBy(p => p.StartMonth)))
            {
                Assert.Equal(periodRequest.StartMonth, period.StartMonth);
                Assert.Equal(periodRequest.StartYear, period.StartYear);
                Assert.Equal(periodRequest.EndMonth, period.EndMonth);
                Assert.Equal(periodRequest.EndYear, period.EndYear);
                Assert.Equal(periodRequest.EpisodeCount, period.EpisodeCount);
                Assert.Equal(periodRequest.IsSingleDayRelease, period.IsSingleDayRelease);
                Assert.Equal(periodRequest.RottenTomatoesId, period.RottenTomatoesId);
            }
        }

        foreach (var (episodeRequest, episode) in request.SpecialEpisodes.OrderBy(e => e.SequenceNumber)
            .Zip(series.SpecialEpisodes.OrderBy(e => e.SequenceNumber)))
        {
            AssertTitles(episodeRequest, episode);

            Assert.Equal(episodeRequest.SequenceNumber, episode.SequenceNumber);
            Assert.Equal(episodeRequest.IsWatched, episode.IsWatched);
            Assert.Equal(episodeRequest.IsReleased, episode.IsReleased);
            Assert.Equal(episodeRequest.Channel, episode.Channel);
            Assert.Equal(episodeRequest.Month, episode.Month);
            Assert.Equal(episodeRequest.Year, episode.Year);
            Assert.Equal(episodeRequest.RottenTomatoesId, episode.RottenTomatoesId);
        }
    }

    [Fact(DisplayName = "AddSeries should return a correct model")]
    public async Task AddSeriesShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.AddSeries(request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(request, model);

        Assert.Equal(request.WatchStatus, model.WatchStatus);
        Assert.Equal(request.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);

        foreach (var (seasonRequest, seasonModel) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(model.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            AssertTitles(seasonRequest, seasonModel);

            Assert.Equal(seasonRequest.SequenceNumber, seasonModel.SequenceNumber);
            Assert.Equal(seasonRequest.WatchStatus, seasonModel.WatchStatus);
            Assert.Equal(seasonRequest.ReleaseStatus, seasonModel.ReleaseStatus);
            Assert.Equal(seasonRequest.Channel, seasonModel.Channel);

            foreach (var (periodRequest, periodModel) in seasonRequest.Periods
                .OrderBy(p => p.StartYear)
                .ThenBy(p => p.StartMonth)
                .Zip(seasonModel.Periods.OrderBy(p => p.StartYear).ThenBy(p => p.StartMonth)))
            {
                Assert.Equal(periodRequest.StartMonth, periodModel.StartMonth);
                Assert.Equal(periodRequest.StartYear, periodModel.StartYear);
                Assert.Equal(periodRequest.EndMonth, periodModel.EndMonth);
                Assert.Equal(periodRequest.EndYear, periodModel.EndYear);
                Assert.Equal(periodRequest.EpisodeCount, periodModel.EpisodeCount);
                Assert.Equal(periodRequest.IsSingleDayRelease, periodModel.IsSingleDayRelease);
                Assert.Equal(periodRequest.RottenTomatoesId, periodModel.RottenTomatoesId);
            }
        }

        foreach (var (episodeRequest, episodeModel) in request.SpecialEpisodes.OrderBy(e => e.SequenceNumber)
            .Zip(model.SpecialEpisodes.OrderBy(e => e.SequenceNumber)))
        {
            AssertTitles(episodeRequest, episodeModel);

            Assert.Equal(episodeRequest.SequenceNumber, episodeModel.SequenceNumber);
            Assert.Equal(episodeRequest.IsWatched, episodeModel.IsWatched);
            Assert.Equal(episodeRequest.IsReleased, episodeModel.IsReleased);
            Assert.Equal(episodeRequest.Channel, episodeModel.Channel);
            Assert.Equal(episodeRequest.Month, episodeModel.Month);
            Assert.Equal(episodeRequest.Year, episodeModel.Year);
            Assert.Equal(episodeRequest.RottenTomatoesId, episodeModel.RottenTomatoesId);
        }
    }

    [Fact(DisplayName = "AddSeries should throw if the kind is not found")]
    public async Task AddSeriesShouldThrowIfKindNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var dummyKindId = Id.Create<SeriesKind>();
        var request = this.CreateSeriesRequest() with { KindId = dummyKindId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.AddSeries(request.Validated(), TestContext.Current.CancellationToken));

        Assert.Equal("Resource.SeriesKind", exception.Resource);
        Assert.Equal(dummyKindId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "UpdateSeries should update it in the database")]
    public async Task UpdateSeriesShouldUpdateItInDb()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var dbSeries = await data.CreateSeries(dbContext);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.UpdateSeries(
            dbSeries.Id, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        var series = dbContext.Series.Find(Id.For<Series>(model.Id));

        Assert.NotNull(series);

        AssertTitles(request, series);

        Assert.Equal(request.WatchStatus, series.WatchStatus);
        Assert.Equal(request.ReleaseStatus, series.ReleaseStatus);
        Assert.Equal(request.KindId, series.Kind.Id.Value);
        Assert.Equal(request.ImdbId, series.ImdbId);
        Assert.Equal(request.RottenTomatoesId, series.RottenTomatoesId);

        foreach (var (seasonRequest, season) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(series.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            AssertTitles(seasonRequest, season);

            Assert.Equal(seasonRequest.SequenceNumber, season.SequenceNumber);
            Assert.Equal(seasonRequest.WatchStatus, season.WatchStatus);
            Assert.Equal(seasonRequest.ReleaseStatus, season.ReleaseStatus);
            Assert.Equal(seasonRequest.Channel, season.Channel);

            foreach (var (periodRequest, period) in seasonRequest.Periods
                .OrderBy(p => p.StartYear)
                .ThenBy(p => p.StartMonth)
                .Zip(season.Periods.OrderBy(p => p.StartYear).ThenBy(p => p.StartMonth)))
            {
                Assert.Equal(periodRequest.StartMonth, period.StartMonth);
                Assert.Equal(periodRequest.StartYear, period.StartYear);
                Assert.Equal(periodRequest.EndMonth, period.EndMonth);
                Assert.Equal(periodRequest.EndYear, period.EndYear);
                Assert.Equal(periodRequest.EpisodeCount, period.EpisodeCount);
                Assert.Equal(periodRequest.IsSingleDayRelease, period.IsSingleDayRelease);
                Assert.Equal(periodRequest.RottenTomatoesId, period.RottenTomatoesId);
            }
        }

        foreach (var (episodeRequest, episode) in request.SpecialEpisodes.OrderBy(e => e.SequenceNumber)
            .Zip(series.SpecialEpisodes.OrderBy(e => e.SequenceNumber)))
        {
            AssertTitles(episodeRequest, episode);

            Assert.Equal(episodeRequest.SequenceNumber, episode.SequenceNumber);
            Assert.Equal(episodeRequest.IsWatched, episode.IsWatched);
            Assert.Equal(episodeRequest.IsReleased, episode.IsReleased);
            Assert.Equal(episodeRequest.Channel, episode.Channel);
            Assert.Equal(episodeRequest.Month, episode.Month);
            Assert.Equal(episodeRequest.Year, episode.Year);
            Assert.Equal(episodeRequest.RottenTomatoesId, episode.RottenTomatoesId);
        }
    }

    [Fact(DisplayName = "UpdateSeries should return a correct model")]
    public async Task UpdateSeriesShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var dbSeries = await data.CreateSeries(dbContext);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.UpdateSeries(
            dbSeries.Id, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(request, model);

        Assert.Equal(request.WatchStatus, model.WatchStatus);
        Assert.Equal(request.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);

        foreach (var (seasonRequest, seasonModel) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(model.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            AssertTitles(seasonRequest, seasonModel);

            Assert.Equal(seasonRequest.SequenceNumber, seasonModel.SequenceNumber);
            Assert.Equal(seasonRequest.WatchStatus, seasonModel.WatchStatus);
            Assert.Equal(seasonRequest.ReleaseStatus, seasonModel.ReleaseStatus);
            Assert.Equal(seasonRequest.Channel, seasonModel.Channel);

            foreach (var (periodRequest, periodModel) in seasonRequest.Periods
                .OrderBy(p => p.StartYear)
                .ThenBy(p => p.StartMonth)
                .Zip(seasonModel.Periods.OrderBy(p => p.StartYear).ThenBy(p => p.StartMonth)))
            {
                Assert.Equal(periodRequest.StartMonth, periodModel.StartMonth);
                Assert.Equal(periodRequest.StartYear, periodModel.StartYear);
                Assert.Equal(periodRequest.EndMonth, periodModel.EndMonth);
                Assert.Equal(periodRequest.EndYear, periodModel.EndYear);
                Assert.Equal(periodRequest.EpisodeCount, periodModel.EpisodeCount);
                Assert.Equal(periodRequest.IsSingleDayRelease, periodModel.IsSingleDayRelease);
                Assert.Equal(periodRequest.RottenTomatoesId, periodModel.RottenTomatoesId);
            }
        }

        foreach (var (episodeRequest, episodeModel) in request.SpecialEpisodes.OrderBy(e => e.SequenceNumber)
            .Zip(model.SpecialEpisodes.OrderBy(e => e.SequenceNumber)))
        {
            AssertTitles(episodeRequest, episodeModel);

            Assert.Equal(episodeRequest.SequenceNumber, episodeModel.SequenceNumber);
            Assert.Equal(episodeRequest.IsWatched, episodeModel.IsWatched);
            Assert.Equal(episodeRequest.IsReleased, episodeModel.IsReleased);
            Assert.Equal(episodeRequest.Channel, episodeModel.Channel);
            Assert.Equal(episodeRequest.Month, episodeModel.Month);
            Assert.Equal(episodeRequest.Year, episodeModel.Year);
            Assert.Equal(episodeRequest.RottenTomatoesId, episodeModel.RottenTomatoesId);
        }
    }

    [Fact(DisplayName = "UpdateSeries should throw if not found")]
    public async Task UpdateSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var dbSeries = await data.CreateSeries(dbContext);

        var request = this.CreateSeriesRequest();
        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.UpdateSeries(dummyId, request.Validated(), TestContext.Current.CancellationToken));

        Assert.Equal("NotFound.Series", exception.MessageCode);
        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "RemoveSeries should remote it from the database")]
    public async Task RemoveSeriesShouldRemoveItFromDb()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var series = await data.CreateSeries(dbContext);

        // Act

        await seriesService.RemoveSeries(series.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.True(dbContext.Series.All(m => m.Id != series.Id));
    }

    [Fact(DisplayName = "RemoveSeries should throw if series isn't found")]
    public async Task RemoveSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, data.PosterValidator, this.logger);

        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.RemoveSeries(dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("NotFound.Series", exception.MessageCode);
        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private SeriesRequest CreateSeriesRequest() =>
        new(
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            data.SeriesKindId.Value,
            ImmutableList.Create(this.CreateSeasonRequest(1), this.CreateSeasonRequest(2)).AsValue(),
            ImmutableList.Create(this.CreateSpecialEpisodeRequest(3)).AsValue(),
            "tt12345678",
            null,
            null);

    private SeasonRequest CreateSeasonRequest(int num) =>
        new(
            null,
            ImmutableList.Create(new TitleRequest($"Season {num}", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest($"Season {num}", 1)).AsValue(),
            num,
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.Finished,
            "Test",
            ImmutableList.Create(new PeriodRequest(null, 1, 2000 + num, 2, 2000 + num, 10, false, null)).AsValue());

    private SpecialEpisodeRequest CreateSpecialEpisodeRequest(int num) =>
        new(
            null,
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            num,
            false,
            true,
            "Test",
            1,
            2000 + num,
            null);
}
