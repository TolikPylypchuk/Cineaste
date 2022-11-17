namespace Cineaste.Server.Services;

using System.Globalization;

using Cineaste.Basic;
using Cineaste.Core.Domain;
using Cineaste.Persistence;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Models.Shared;
using Cineaste.Shared.Validation;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public class SeriesServiceTests : ServiceTestsBase
{
    private readonly CineasteList list;
    private readonly SeriesKind kind;
    private readonly ILogger<SeriesService> logger;

    public SeriesServiceTests()
    {
        this.list = this.CreateList();
        this.kind = this.CreateKind(this.list);
        this.logger = new NullLogger<SeriesService>();
    }

    [Fact(DisplayName = "GetSeries should return the correct series")]
    public async Task GetSeriesShouldReturnCorrectSeries()
    {
        var dbContext = this.CreateInMemoryDb();
        var seriesService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var series = this.CreateSeries();
        this.list.AddSeries(series);

        dbContext.Series.Add(series);
        dbContext.SaveChanges();

        var model = await seriesService.GetSeries(series.Id);

        this.AssertTitles(series, model);

        Assert.Equal(series.Id.Value, model.Id);
        Assert.Equal(series.WatchStatus, model.WatchStatus);
        Assert.Equal(series.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(series.Kind.Id.Value, model.Kind.Id);
        Assert.Equal(series.ImdbId, model.ImdbId);
        Assert.Equal(series.RottenTomatoesId, model.RottenTomatoesId);

        foreach (var (season, seasonModel) in series.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(model.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            this.AssertTitles(season, seasonModel);

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
            this.AssertTitles(episode, episodeModel);

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
        var dbContext = this.CreateInMemoryDb();
        var seriesService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyId = Id.CreateNew<Series>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.GetSeries(dummyId));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateSeries should put it into the database")]
    public async Task CreateSeriesShouldPutItIntoDb()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var request = this.CreateSeriesRequest();

        var model = await movieService.CreateSeries(request.Validated());

        var series = dbContext.Series.Find(Id.Create<Series>(model.Id));

        Assert.NotNull(series);

        this.AssertTitles(request, series);

        Assert.Equal(request.WatchStatus, series.WatchStatus);
        Assert.Equal(request.ReleaseStatus, series.ReleaseStatus);
        Assert.Equal(request.KindId, series.Kind.Id.Value);
        Assert.Equal(request.ImdbId, series.ImdbId);
        Assert.Equal(request.RottenTomatoesId, series.RottenTomatoesId);

        foreach (var (seasonRequest, season) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(series.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            this.AssertTitles(seasonRequest, season);

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
            this.AssertTitles(episodeRequest, episode);

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
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var request = this.CreateSeriesRequest();

        var model = await movieService.CreateSeries(request.Validated());

        this.AssertTitles(request, model);

        Assert.Equal(request.WatchStatus, model.WatchStatus);
        Assert.Equal(request.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);

        foreach (var (seasonRequest, seasonModel) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(model.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            this.AssertTitles(seasonRequest, seasonModel);

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
            this.AssertTitles(episodeRequest, episodeModel);

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
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyListId = Id.CreateNew<CineasteList>();
        var request = this.CreateSeriesRequest() with { ListId = dummyListId.Value };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.CreateSeries(request.Validated()));

        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyListId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateSeries should throw if the kind is not found")]
    public async Task CreateSeriesShouldThrowIfKindNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyKindId = Id.CreateNew<SeriesKind>();
        var request = this.CreateSeriesRequest() with { KindId = dummyKindId.Value };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.CreateSeries(request.Validated()));

        Assert.Equal("Resource.SeriesKind", exception.Resource);
        Assert.Equal(dummyKindId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateSeries should throw if the kind doesn't belong to list")]
    public async Task CreateSeriesShouldThrowIfKindDoesNotBelongToList()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var otherList = this.CreateList();
        var otherKind = this.CreateKind(otherList);

        dbContext.SeriesKinds.Add(otherKind);
        dbContext.Lists.Add(otherList);
        dbContext.SaveChanges();

        var request = this.CreateSeriesRequest() with { KindId = otherKind.Id.Value };

        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            movieService.CreateSeries(request.Validated()));

        Assert.Equal("BadRequest.SeriesKind.WrongList", exception.MessageCode);
        Assert.Equal(this.list.Id, exception.Properties["listId"]);
        Assert.Equal(otherKind.Id, exception.Properties["kindId"]);
    }

    [Fact(DisplayName = "UpdateSeries should update it in the database")]
    public async Task UpdateSeriesShouldUpdateItInDb()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dbSeries = this.CreateSeries();
        this.list.AddSeries(dbSeries);

        dbContext.Series.Add(dbSeries);
        dbContext.SaveChanges();

        var request = this.CreateSeriesRequest();

        var model = await movieService.UpdateSeries(dbSeries.Id, request.Validated());

        var series = dbContext.Series.Find(Id.Create<Series>(model.Id));

        Assert.NotNull(series);

        this.AssertTitles(request, series);

        Assert.Equal(request.WatchStatus, series.WatchStatus);
        Assert.Equal(request.ReleaseStatus, series.ReleaseStatus);
        Assert.Equal(request.KindId, series.Kind.Id.Value);
        Assert.Equal(request.ImdbId, series.ImdbId);
        Assert.Equal(request.RottenTomatoesId, series.RottenTomatoesId);

        foreach (var (seasonRequest, season) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(series.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            this.AssertTitles(seasonRequest, season);

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
            this.AssertTitles(episodeRequest, episode);

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
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dbSeries = this.CreateSeries();
        this.list.AddSeries(dbSeries);

        dbContext.Series.Add(dbSeries);
        dbContext.SaveChanges();

        var request = this.CreateSeriesRequest();

        var model = await movieService.UpdateSeries(dbSeries.Id, request.Validated());

        this.AssertTitles(request, model);

        Assert.Equal(request.WatchStatus, model.WatchStatus);
        Assert.Equal(request.ReleaseStatus, model.ReleaseStatus);
        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.ImdbId, model.ImdbId);
        Assert.Equal(request.RottenTomatoesId, model.RottenTomatoesId);

        foreach (var (seasonRequest, seasonModel) in request.Seasons.OrderBy(s => s.SequenceNumber)
            .Zip(model.Seasons.OrderBy(s => s.SequenceNumber)))
        {
            this.AssertTitles(seasonRequest, seasonModel);

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
            this.AssertTitles(episodeRequest, episodeModel);

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
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var series = this.CreateSeries();
        this.list.AddSeries(series);

        dbContext.Series.Add(series);
        dbContext.SaveChanges();

        var request = this.CreateSeriesRequest();
        var dummyId = Id.CreateNew<Series>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            movieService.UpdateSeries(dummyId, request.Validated()));

        Assert.Equal("NotFound.Series", exception.MessageCode);
        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "UpdateSeries should throw if series doesn't belong to list")]
    public async Task UpdateSeriesShouldThrowIfMovieDoesNotBelongToList()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var series = this.CreateSeries();
        this.list.AddSeries(series);

        dbContext.Series.Add(series);

        var otherList = this.CreateList();
        dbContext.Lists.Add(otherList);

        dbContext.SaveChanges();

        var request = this.CreateSeriesRequest() with { ListId = otherList.Id.Value };

        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            movieService.UpdateSeries(series.Id, request.Validated()));

        Assert.Equal("BadRequest.Series.WrongList", exception.MessageCode);
        Assert.Equal(series.Id, exception.Properties["seriesId"]);
        Assert.Equal(otherList.Id, exception.Properties["listId"]);
    }

    [Fact(DisplayName = "DeleteSeries should remote it from the database")]
    public async Task DeleteSeriesShouldRemoveItFromDb()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var series = this.CreateSeries();
        this.list.AddSeries(series);

        dbContext.Series.Add(series);
        dbContext.SaveChanges();

        await movieService.DeleteSeries(series.Id);

        Assert.True(dbContext.Series.All(m => m.Id != series.Id));
    }

    [Fact(DisplayName = "DeleteSeries should throw if series isn't found")]
    public async Task DeleteSeriesShouldThrowIfNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var movieService = new SeriesService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyId = Id.CreateNew<Series>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => movieService.DeleteSeries(dummyId));

        Assert.Equal("NotFound.Series", exception.MessageCode);
        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private Series CreateSeries() =>
        new(
            Id.CreateNew<Series>(),
            this.CreateTitles(),
            new List<Season> { this.CreateSeason(1), this.CreateSeason(2) },
            new List<SpecialEpisode> { this.CreateSpecialEpisode(3) },
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            this.kind);

    private Season CreateSeason(int num) =>
        new(
            Id.CreateNew<Season>(),
            new List<Title> { new($"Season {num}", 1, false), new($"Season {num}", 1, true) },
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.Finished,
            "Test",
            num,
            new List<Period>
            {
                new(Id.CreateNew<Period>(), 1, 2000 + num, 2, 2000 + num, false, 10)
            });

    private SpecialEpisode CreateSpecialEpisode(int num) =>
        new(Id.CreateNew<SpecialEpisode>(), this.CreateTitles(), 1, 2000 + num, false, true, "Test", num);

    private IEnumerable<Title> CreateTitles() =>
        new List<Title> { new("Test", 1, false), new("Original Test", 1, true) };

    private SeriesRequest CreateSeriesRequest() =>
        new(
            list.Id.Value,
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Original Test", 1)).AsValue(),
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            this.kind.Id.Value,
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

    private SeriesKind CreateKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new SeriesKind(Id.CreateNew<SeriesKind>(), "Test Kind", black, black, black);

        list.AddSeriesKind(kind);

        return kind;
    }

    private void SetUpDb(CineasteDbContext context)
    {
        context.SeriesKinds.Add(this.kind);
        context.Lists.Add(this.list);

        context.SaveChanges();
    }
}
