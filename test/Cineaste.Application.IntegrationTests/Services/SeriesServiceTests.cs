using Cineaste.Shared.Models.Series;

namespace Cineaste.Application.Services;

public class SeriesServiceTests(DbFixture dbFixture, ITestOutputHelper output)
    : TestClassBase(dbFixture)
{
    private readonly ILogger<SeriesService> logger = XUnitLogger.Create<SeriesService>(output);

    [Fact(DisplayName = "GetSeries should return the correct series")]
    public async Task GetSeriesShouldReturnCorrectSeries()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);

        // Act

        var model = await seriesService.GetSeries(this.data.ListId, series.Id, TestContext.Current.CancellationToken);

        // Assert

        AssertTitles(series, model);

        Assert.Equal(series.Id.Value, model.Id);
        Assert.Equal(series.WatchStatus, model.WatchStatus);
        Assert.Equal(series.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(series.Kind.Id.Value, model.Kind.Id);
        Assert.Equal(series.ImdbId?.Value, model.ImdbId);
        Assert.Equal(series.RottenTomatoesId?.Value, model.RottenTomatoesId);

        foreach (var (season, seasonModel) in series.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(model.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            AssertTitles(season, seasonModel);

            Assert.Equal(season.SequenceNumber, seasonModel.SequenceNumber);
            Assert.Equal(season.WatchStatus, seasonModel.WatchStatus);
            Assert.Equal(season.ReleaseStatus, seasonModel.ReleaseStatus);
            Assert.Equal(season.Channel, seasonModel.Channel);

            foreach (var (part, partModel) in season.Parts
                .OrderBy(p => p.Period.StartYear)
                .ThenBy(p => p.Period.StartMonth)
                .Zip(seasonModel.Parts.OrderBy(p => p.Period.StartYear).ThenBy(p => p.Period.StartMonth)))
            {
                Assert.Equal(part.Period.StartMonth.Value, partModel.Period.StartMonth);
                Assert.Equal(part.Period.StartYear, partModel.Period.StartYear);
                Assert.Equal(part.Period.EndMonth.Value, partModel.Period.EndMonth);
                Assert.Equal(part.Period.EndYear, partModel.Period.EndYear);
                Assert.Equal(part.Period.EpisodeCount, partModel.Period.EpisodeCount);
                Assert.Equal(part.Period.IsSingleDayRelease, partModel.Period.IsSingleDayRelease);
                Assert.Equal(part.RottenTomatoesId?.Value, partModel.RottenTomatoesId);
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
            Assert.Equal(episode.Month.Value, episodeModel.Month);
            Assert.Equal(episode.Year, episodeModel.Year);
            Assert.Equal(episode.RottenTomatoesId?.Value, episodeModel.RottenTomatoesId);
        }
    }

    [Fact(DisplayName = "GetSeries should throw if series isn't found")]
    public async Task GetSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.GetSeries(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.SeriesId);
    }

    [Fact(DisplayName = "AddSeries should put it into the database")]
    public async Task AddSeriesShouldPutItIntoDb()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.AddSeries(
            this.data.ListId, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        var series = dbContext.Series.Find(Id.For<Series>(model.Id));

        Assert.NotNull(series);

        AssertTitles(request, series);

        Assert.Equal(request.WatchStatus, series.WatchStatus);
        Assert.Equal(request.ReleaseStatus, series.ReleaseStatus);
        Assert.Equal(request.KindId, series.Kind.Id.Value);
        Assert.Equal(request.ImdbId, series.ImdbId?.Value);
        Assert.Equal(request.RottenTomatoesId, series.RottenTomatoesId?.Value);

        foreach (var (seasonRequest, season) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(series.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            AssertTitles(seasonRequest, season);

            Assert.Equal(seasonRequest.SequenceNumber, season.SequenceNumber);
            Assert.Equal(seasonRequest.WatchStatus, season.WatchStatus);
            Assert.Equal(seasonRequest.ReleaseStatus, season.ReleaseStatus);
            Assert.Equal(seasonRequest.Channel, season.Channel);

            foreach (var (partRequest, part) in seasonRequest.Parts
                .OrderBy(p => p.Period.StartYear)
                .ThenBy(p => p.Period.StartMonth)
                .Zip(season.Parts.OrderBy(p => p.Period.StartYear).ThenBy(p => p.Period.StartMonth)))
            {
                Assert.Equal(partRequest.Period.StartMonth, part.Period.StartMonth.Value);
                Assert.Equal(partRequest.Period.StartYear, part.Period.StartYear);
                Assert.Equal(partRequest.Period.EndMonth, part.Period.EndMonth.Value);
                Assert.Equal(partRequest.Period.EndYear, part.Period.EndYear);
                Assert.Equal(partRequest.Period.EpisodeCount, part.Period.EpisodeCount);
                Assert.Equal(partRequest.Period.IsSingleDayRelease, part.Period.IsSingleDayRelease);
                Assert.Equal(partRequest.RottenTomatoesId, part.RottenTomatoesId?.Value);
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
            Assert.Equal(episodeRequest.Month, episode.Month.Value);
            Assert.Equal(episodeRequest.Year, episode.Year);
            Assert.Equal(episodeRequest.RottenTomatoesId, episode.RottenTomatoesId?.Value);
        }
    }

    [Fact(DisplayName = "AddSeries should return a correct model")]
    public async Task AddSeriesShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.AddSeries(
            this.data.ListId, request.Validated(), TestContext.Current.CancellationToken);

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

            foreach (var (partRequest, partModel) in seasonRequest.Parts
                .OrderBy(p => p.Period.StartYear)
                .ThenBy(p => p.Period.StartMonth)
                .Zip(seasonModel.Parts.OrderBy(p => p.Period.StartYear).ThenBy(p => p.Period.StartMonth)))
            {
                Assert.Equal(partRequest.Period.StartMonth, partModel.Period.StartMonth);
                Assert.Equal(partRequest.Period.StartYear, partModel.Period.StartYear);
                Assert.Equal(partRequest.Period.EndMonth, partModel.Period.EndMonth);
                Assert.Equal(partRequest.Period.EndYear, partModel.Period.EndYear);
                Assert.Equal(partRequest.Period.EpisodeCount, partModel.Period.EpisodeCount);
                Assert.Equal(partRequest.Period.IsSingleDayRelease, partModel.Period.IsSingleDayRelease);
                Assert.Equal(partRequest.RottenTomatoesId, partModel.RottenTomatoesId);
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

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummyKindId = Id.Create<SeriesKind>();
        var request = this.CreateSeriesRequest() with { KindId = dummyKindId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesKindNotFoundException>(() =>
            seriesService.AddSeries(this.data.ListId, request.Validated(), TestContext.Current.CancellationToken));

        Assert.Equal(dummyKindId, exception.KindId);
    }

    [Fact(DisplayName = "UpdateSeries should update it in the database")]
    public async Task UpdateSeriesShouldUpdateItInDb()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dbSeries = await this.data.CreateSeries(dbContext);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.UpdateSeries(
            this.data.ListId, dbSeries.Id, request.Validated(), TestContext.Current.CancellationToken);

        // Assert

        var series = dbContext.Series.Find(Id.For<Series>(model.Id));

        Assert.NotNull(series);

        AssertTitles(request, series);

        Assert.Equal(request.WatchStatus, series.WatchStatus);
        Assert.Equal(request.ReleaseStatus, series.ReleaseStatus);
        Assert.Equal(request.KindId, series.Kind.Id.Value);
        Assert.Equal(request.ImdbId, series.ImdbId?.Value);
        Assert.Equal(request.RottenTomatoesId, series.RottenTomatoesId?.Value);

        foreach (var (seasonRequest, season) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(series.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            AssertTitles(seasonRequest, season);

            Assert.Equal(seasonRequest.SequenceNumber, season.SequenceNumber);
            Assert.Equal(seasonRequest.WatchStatus, season.WatchStatus);
            Assert.Equal(seasonRequest.ReleaseStatus, season.ReleaseStatus);
            Assert.Equal(seasonRequest.Channel, season.Channel);

            foreach (var (periodRequest, period) in seasonRequest.Parts
                .OrderBy(p => p.Period.StartYear)
                .ThenBy(p => p.Period.StartMonth)
                .Zip(season.Parts.OrderBy(p => p.Period.StartYear).ThenBy(p => p.Period.StartMonth)))
            {
                Assert.Equal(periodRequest.Period.StartMonth, period.Period.StartMonth.Value);
                Assert.Equal(periodRequest.Period.StartYear, period.Period.StartYear);
                Assert.Equal(periodRequest.Period.EndMonth, period.Period.EndMonth.Value);
                Assert.Equal(periodRequest.Period.EndYear, period.Period.EndYear);
                Assert.Equal(periodRequest.Period.EpisodeCount, period.Period.EpisodeCount);
                Assert.Equal(periodRequest.Period.IsSingleDayRelease, period.Period.IsSingleDayRelease);
                Assert.Equal(periodRequest.RottenTomatoesId, period.RottenTomatoesId?.Value);
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
            Assert.Equal(episodeRequest.Month, episode.Month.Value);
            Assert.Equal(episodeRequest.Year, episode.Year);
            Assert.Equal(episodeRequest.RottenTomatoesId, episode.RottenTomatoesId?.Value);
        }
    }

    [Fact(DisplayName = "UpdateSeries should return a correct model")]
    public async Task UpdateSeriesShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dbSeries = await this.data.CreateSeries(dbContext);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.UpdateSeries(
            this.data.ListId, dbSeries.Id, request.Validated(), TestContext.Current.CancellationToken);

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

            foreach (var (partRequest, partModel) in seasonRequest.Parts
                .OrderBy(p => p.Period.StartYear)
                .ThenBy(p => p.Period.StartMonth)
                .Zip(seasonModel.Parts.OrderBy(p => p.Period.StartYear).ThenBy(p => p.Period.StartMonth)))
            {
                Assert.Equal(partRequest.Period.StartMonth, partModel.Period.StartMonth);
                Assert.Equal(partRequest.Period.StartYear, partModel.Period.StartYear);
                Assert.Equal(partRequest.Period.EndMonth, partModel.Period.EndMonth);
                Assert.Equal(partRequest.Period.EndYear, partModel.Period.EndYear);
                Assert.Equal(partRequest.Period.EpisodeCount, partModel.Period.EpisodeCount);
                Assert.Equal(partRequest.Period.IsSingleDayRelease, partModel.Period.IsSingleDayRelease);
                Assert.Equal(partRequest.RottenTomatoesId, partModel.RottenTomatoesId);
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

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dbSeries = await this.data.CreateSeries(dbContext);

        var request = this.CreateSeriesRequest();
        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.UpdateSeries(
                this.data.ListId, dummyId, request.Validated(), TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.SeriesId);
    }

    [Fact(DisplayName = "RemoveSeries should remote it from the database")]
    public async Task RemoveSeriesShouldRemoveItFromDb()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);

        // Act

        await seriesService.RemoveSeries(this.data.ListId, series.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.True(dbContext.Series.All(m => m.Id != series.Id));
    }

    [Fact(DisplayName = "RemoveSeries should throw if series isn't found")]
    public async Task RemoveSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.RemoveSeries(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.SeriesId);
    }

    private SeriesService CreateSeriesService(CineasteDbContext dbContext) =>
        new(dbContext, this.data.PosterProvider, this.data.PosterUrlProvider, this.logger);

    private SeriesRequest CreateSeriesRequest() =>
        new(
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            this.data.SeriesKindId.Value,
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
            ImmutableList.Create(new SeasonPartRequest(
                null, new(1, 2000 + num, 2, 2000 + num, 10, false), null)).AsValue());

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
