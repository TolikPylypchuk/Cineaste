namespace Cineaste.Server.Services;

using Cineaste.Basic;
using Cineaste.Core.Domain;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Models.Shared;
using Cineaste.Shared.Validation;

using Microsoft.Extensions.Logging;

public class SeriesServiceTests : ServiceTestsBase
{
    private readonly ILogger<SeriesService> logger;

    public SeriesServiceTests(ITestOutputHelper output) =>
        this.logger = XUnitLogger.Create<SeriesService>(output);

    [Fact(DisplayName = "GetSeries should return the correct series")]
    public async Task GetSeriesShouldReturnCorrectSeries()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var series = this.CreateSeries(this.List);

        dbContext.Series.Add(series);
        await dbContext.SaveChangesAsync();

        // Act

        var model = await seriesService.GetSeries(series.Id);

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

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.GetSeries(dummyId));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateSeries should put it into the database")]
    public async Task CreateSeriesShouldPutItIntoDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.CreateSeries(request.Validated());

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

    [Fact(DisplayName = "CreateSeries should return a correct model")]
    public async Task CreateSeriesShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.CreateSeries(request.Validated());

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

    [Fact(DisplayName = "CreateSeries should throw if the list is not found")]
    public async Task CreateSeriesShouldThrowIfListNotFound()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var dummyListId = Id.Create<CineasteList>();
        var request = this.CreateSeriesRequest() with { ListId = dummyListId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.CreateSeries(request.Validated()));

        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyListId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateSeries should throw if the kind is not found")]
    public async Task CreateSeriesShouldThrowIfKindNotFound()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var dummyKindId = Id.Create<SeriesKind>();
        var request = this.CreateSeriesRequest() with { KindId = dummyKindId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.CreateSeries(request.Validated()));

        Assert.Equal("Resource.SeriesKind", exception.Resource);
        Assert.Equal(dummyKindId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateSeries should throw if the kind doesn't belong to list")]
    public async Task CreateSeriesShouldThrowIfKindDoesNotBelongToList()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var otherList = this.CreateList("Other List for Series Kind", "other-list-for-series-kind");
        var otherKind = this.CreateSeriesKind(otherList);

        dbContext.SeriesKinds.Add(otherKind);
        dbContext.Lists.Add(otherList);
        await dbContext.SaveChangesAsync();

        var request = this.CreateSeriesRequest() with { KindId = otherKind.Id.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            seriesService.CreateSeries(request.Validated()));

        Assert.Equal("BadRequest.SeriesKind.WrongList", exception.MessageCode);
        Assert.Equal(this.List.Id, exception.Properties["listId"]);
        Assert.Equal(otherKind.Id, exception.Properties["kindId"]);
    }

    [Fact(DisplayName = "UpdateSeries should update it in the database")]
    public async Task UpdateSeriesShouldUpdateItInDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var dbSeries = this.CreateSeries(this.List);

        dbContext.Series.Add(dbSeries);
        await dbContext.SaveChangesAsync();

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.UpdateSeries(dbSeries.Id, request.Validated());

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

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var dbSeries = this.CreateSeries(this.List);

        dbContext.Series.Add(dbSeries);
        await dbContext.SaveChangesAsync();

        var request = this.CreateSeriesRequest();

        // Act

        var model = await seriesService.UpdateSeries(dbSeries.Id, request.Validated());

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

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var series = this.CreateSeries(this.List);

        dbContext.Series.Add(series);
        await dbContext.SaveChangesAsync();

        var request = this.CreateSeriesRequest();
        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.UpdateSeries(dummyId, request.Validated()));

        Assert.Equal("NotFound.Series", exception.MessageCode);
        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "UpdateSeries should throw if series doesn't belong to list")]
    public async Task UpdateSeriesShouldThrowIfSeriesDoesNotBelongToList()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var series = this.CreateSeries(this.List);

        dbContext.Series.Add(series);

        var otherList = this.CreateList("Other List for Series", "other-list-for-series");
        dbContext.Lists.Add(otherList);

        await dbContext.SaveChangesAsync();

        var request = this.CreateSeriesRequest() with { ListId = otherList.Id.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            seriesService.UpdateSeries(series.Id, request.Validated()));

        Assert.Equal("BadRequest.Series.WrongList", exception.MessageCode);
        Assert.Equal(series.Id, exception.Properties["seriesId"]);
        Assert.Equal(otherList.Id, exception.Properties["listId"]);
    }

    [Fact(DisplayName = "DeleteSeries should remote it from the database")]
    public async Task DeleteSeriesShouldRemoveItFromDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var series = this.CreateSeries(this.List);

        dbContext.Series.Add(series);
        await dbContext.SaveChangesAsync();

        // Act

        await seriesService.DeleteSeries(series.Id);

        // Assert

        Assert.True(dbContext.Series.All(m => m.Id != series.Id));
    }

    [Fact(DisplayName = "DeleteSeries should throw if series isn't found")]
    public async Task DeleteSeriesShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var seriesService = new SeriesService(dbContext, this.logger);

        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.DeleteSeries(dummyId));

        Assert.Equal("NotFound.Series", exception.MessageCode);
        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private SeriesRequest CreateSeriesRequest() =>
        new(
            this.List.Id.Value,
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            this.SeriesKind.Id.Value,
            ImmutableList.Create(this.CreateSeasonRequest(1), this.CreateSeasonRequest(2)).AsValue(),
            ImmutableList.Create(this.CreateSpecialEpisodeRequest(3)).AsValue(),
            "tt12345678",
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
