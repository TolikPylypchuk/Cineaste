using System.Security.Cryptography;

namespace Cineaste.Application.Services;

public sealed class SeriesServicePosterTests(DbFixture dbFixture, ITestOutputHelper output)
    : TestClassBase(dbFixture)
{
    private readonly ILogger<SeriesService> logger = XUnitLogger.Create<SeriesService>(output);

    [Fact(DisplayName = "GetSeriesPoster should get the series poster")]
    public async Task GetSeriesPoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var poster = await this.data.CreateSeriesPoster(series, dbContext);

        // Act

        var posterContent = await seriesService.GetSeriesPoster(
            this.data.ListId, series.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetSeriesPoster should throw if series isn't found")]
    public async Task GetSeriesPosterSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummyId = Id.Create<Series>();

        // Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.GetSeriesPoster(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.SeriesId);
    }

    [Fact(DisplayName = "GetSeriesPoster should throw if poster isn't found")]
    public async Task GetSeriesPosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);

        // Assert

        var exception = await Assert.ThrowsAsync<SeriesPosterNotFoundException>(() =>
            seriesService.GetSeriesPoster(this.data.ListId, series.Id, TestContext.Current.CancellationToken));

        Assert.Equal(series.Id, exception.SeriesId);
    }

    [Fact(DisplayName = "GetSeasonPoster should get the season poster")]
    public async Task GetSeasonPoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await this.data.CreateSeasonPoster(period, dbContext);

        // Act

        var posterContent = await seriesService.GetSeasonPoster(
            this.data.ListId, series.Id, period.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetSeasonPoster should throw if series isn't found")]
    public async Task GetSeasonPosterSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.GetSeasonPoster(
                this.data.ListId, dummySeriesId, dummyPeriodId, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);
    }

    [Fact(DisplayName = "GetSeasonPoster should throw if period isn't found")]
    public async Task GetSeasonPosterPeriodNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var dummyId = Id.Create<Period>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<PeriodNotFoundException>(() =>
            seriesService.GetSeasonPoster(this.data.ListId, series.Id, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.PeriodId);
    }

    [Fact(DisplayName = "GetSeasonPoster should throw if poster isn't found")]
    public async Task GetSeasonPosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeasonPosterNotFoundException>(() =>
            seriesService.GetSeasonPoster(
                this.data.ListId, series.Id, period.Id, TestContext.Current.CancellationToken));

        Assert.Equal(period.Id, exception.PeriodId);
    }

    [Fact(DisplayName = "GetSpecialEpisodePoster should get the special episode poster")]
    public async Task GetSpecialEpisodePoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await this.data.CreateSpecialEpisodePoster(episode, dbContext);

        // Act

        var posterContent = await seriesService.GetSpecialEpisodePoster(
            this.data.ListId, series.Id, episode.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetSpecialEpisodePoster should throw if series isn't found")]
    public async Task GetSpecialEpisodePosterSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() => seriesService.GetSpecialEpisodePoster(
            this.data.ListId, dummySeriesId, dummyEpisodeId, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);
    }

    [Fact(DisplayName = "GetSpecialEpisodePoster should throw if episode isn't found")]
    public async Task GetSpecialEpisodePosterEpisodeNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var dummyId = Id.Create<SpecialEpisode>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SpecialEpisodeNotFoundException>(() =>
            seriesService.GetSpecialEpisodePoster(
                this.data.ListId, series.Id, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.EpisodeId);
    }

    [Fact(DisplayName = "GetSpecialEpisodePoster should throw if poster isn't found")]
    public async Task GetSpecialEpisodePosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SpecialEpisodePosterNotFoundException>(() =>
            seriesService.GetSpecialEpisodePoster(
                this.data.ListId, series.Id, episode.Id, TestContext.Current.CancellationToken));

        Assert.Equal(episode.Id, exception.EpisodeId);
    }

    [Fact(DisplayName = "SetSeriesPoster should set the series poster from a stream")]
    public async Task SetSeriesPosterStream()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var poster = await this.data.CreateSeriesPoster(series, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await seriesService.SetSeriesPoster(
            this.data.ListId, series.Id, posterData, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SeriesPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SeriesPosters.FirstOrDefault(p => p.Series == series);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSeriesPoster from stream should throw if the series isn't found")]
    public async Task SetSeriesPosterStreamSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummyId = Id.Create<Series>();
        var posterData = this.data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.SetSeriesPoster(
                this.data.ListId, dummyId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.SeriesId);
    }

    [Fact(DisplayName = "SetSeriesPoster should set the series poster from a URL")]
    public async Task SetSeriesPosterUrl()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var poster = await this.data.CreateSeriesPoster(series, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterUrlRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSeriesPoster(
            this.data.ListId, series.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SeriesPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SeriesPosters.FirstOrDefault(p => p.Series == series);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSeriesPoster from a URL should throw if the series isn't found")]
    public async Task SetSeriesPosterUrlSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummyId = Id.Create<Series>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.SetSeriesPoster(this.data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.SeriesId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSeriesPoster should set the series poster from IMDb media")]
    public async Task SetSeriesPosterImdbSeriesMedia()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var poster = await this.data.CreateSeriesPoster(series, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterImdbMediaRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSeriesPoster(
            this.data.ListId, series.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SeriesPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SeriesPosters.FirstOrDefault(p => p.Series == series);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSeriesPoster from IMDb media should throw if the series isn't found")]
    public async Task SetSeriesPosterImdbMediaSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummyId = Id.Create<Series>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.SetSeriesPoster(this.data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.SeriesId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSeasonPoster should set the season poster from a stream")]
    public async Task SetSeasonPosterStream()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await this.data.CreateSeasonPoster(period, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await seriesService.SetSeasonPoster(
            this.data.ListId, series.Id, period.Id, posterData, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SeasonPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SeasonPosters.FirstOrDefault(p => p.Period == period);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSeasonPoster from stream should throw if the series isn't found")]
    public async Task SetSeasonPosterStreamSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();
        var posterData = this.data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() => seriesService.SetSeasonPoster(
            this.data.ListId, dummySeriesId, dummyPeriodId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);
    }

    [Fact(DisplayName = "SetSeasonPoster from stream should throw if the period isn't found")]
    public async Task SetSeasonPosterStreamPeriodNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var dummyPeriodId = Id.Create<Period>();
        var posterData = this.data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<PeriodNotFoundException>(() => seriesService.SetSeasonPoster(
            this.data.ListId, series.Id, dummyPeriodId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal(dummyPeriodId, exception.PeriodId);
    }

    [Fact(DisplayName = "SetSeasonPoster should set the season poster from a URL")]
    public async Task SetSeasonPosterUrl()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await this.data.CreateSeasonPoster(period, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterUrlRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSeasonPoster(
            this.data.ListId, series.Id, period.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SeasonPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SeasonPosters.FirstOrDefault(p => p.Period == period);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSeasonPoster from a URL should throw if the series isn't found")]
    public async Task SetSeasonPosterUrlSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() => seriesService.SetSeasonPoster(
            this.data.ListId, dummySeriesId, dummyPeriodId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSeasonPoster from a URL should throw if the period isn't found")]
    public async Task SetSeasonPosterUrlPeriodNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var dummyPeriodId = Id.Create<Period>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<PeriodNotFoundException>(() => seriesService.SetSeasonPoster(
            this.data.ListId, series.Id, dummyPeriodId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyPeriodId, exception.PeriodId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSeasonPoster should set the season poster from IMDb media")]
    public async Task SetSeasonPosterImdbSeriesMedia()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await this.data.CreateSeasonPoster(period, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterImdbMediaRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSeasonPoster(
            this.data.ListId, series.Id, period.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SeasonPosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SeasonPosters.FirstOrDefault(p => p.Period == period);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSeasonPoster from IMDb media should throw if the series isn't found")]
    public async Task SetSeasonPosterImdbMediaSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() => seriesService.SetSeasonPoster(
            this.data.ListId, dummySeriesId, dummyPeriodId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster should set the special episode poster from a stream")]
    public async Task SetSpecialEpisodePosterStream()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await this.data.CreateSpecialEpisodePoster(episode, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await seriesService.SetSpecialEpisodePoster(
            this.data.ListId, series.Id, episode.Id, posterData, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SpecialEpisodePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SpecialEpisodePosters.FirstOrDefault(p => p.SpecialEpisode == episode);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster from stream should throw if the series isn't found")]
    public async Task SetSpecialEpisodePosterStreamSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = this.data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() => seriesService.SetSpecialEpisodePoster(
            this.data.ListId, dummySeriesId, dummyEpisodeId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster from stream should throw if the special episode isn't found")]
    public async Task SetSpecialEpisodePosterStreamPeriodNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = this.data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SpecialEpisodeNotFoundException>(() =>
            seriesService.SetSpecialEpisodePoster(
                this.data.ListId, series.Id, dummyEpisodeId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal(dummyEpisodeId, exception.EpisodeId);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster should set the special episode poster from a URL")]
    public async Task SetSpecialEpisodePosterUrl()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await this.data.CreateSpecialEpisodePoster(episode, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterUrlRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSpecialEpisodePoster(
            this.data.ListId, series.Id, episode.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SpecialEpisodePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SpecialEpisodePosters.FirstOrDefault(p => p.SpecialEpisode == episode);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster from a URL should throw if the series isn't found")]
    public async Task SetSpecialEpisodePosterUrlSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() => seriesService.SetSpecialEpisodePoster(
            this.data.ListId, dummySeriesId, dummyEpisodeId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster from a URL should throw if the special episode isn't found")]
    public async Task SetSpecialEpisodePosterUrlPeriodNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SpecialEpisodeNotFoundException>(() =>
            seriesService.SetSpecialEpisodePoster(
                this.data.ListId, series.Id, dummyEpisodeId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyEpisodeId, exception.EpisodeId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster should set the special episode poster from IMDb media")]
    public async Task SetSpecialEpisodePosterImdbSeriesMedia()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await this.data.CreateSpecialEpisodePoster(episode, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterImdbMediaRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSpecialEpisodePoster(
            this.data.ListId, series.Id, episode.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SpecialEpisodePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SpecialEpisodePosters.FirstOrDefault(p => p.SpecialEpisode == episode);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster from IMDb media should throw if the series isn't found")]
    public async Task SetSpecialEpisodePosterImdbMediaSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() => seriesService.SetSpecialEpisodePoster(
            this.data.ListId, dummySeriesId, dummyEpisodeId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "RemoveSeriesPoster should remove the series poster")]
    public async Task RemoveSeriesPoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var poster = await this.data.CreateSeriesPoster(series, dbContext);

        // Act

        await seriesService.RemoveSeriesPoster(this.data.ListId, series.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.SeriesPosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveSeriesPoster should not throw if poster isn't found")]
    public async Task RemoveSeriesPosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);

        // Act + Assert

        var exception = await Record.ExceptionAsync(() =>
            seriesService.RemoveSeriesPoster(this.data.ListId, series.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveSeriesPoster should throw if the series isn't found")]
    public async Task RemoveSeriesPosterSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.RemoveSeriesPoster(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.SeriesId);
    }

    [Fact(DisplayName = "RemoveSeasonPoster should remove the series poster")]
    public async Task RemoveSeasonPoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await this.data.CreateSeasonPoster(period, dbContext);

        // Act

        await seriesService.RemoveSeasonPoster(
            this.data.ListId, series.Id, period.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.SeasonPosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveSeasonPoster should not throw if poster isn't found")]
    public async Task RemoveSeasonPosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();

        // Act + Assert

        var exception = await Record.ExceptionAsync(() => seriesService.RemoveSeasonPoster(
            this.data.ListId, series.Id, period.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveSeasonPoster should throw if the series isn't found")]
    public async Task RemoveSeasonPosterSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() => seriesService.RemoveSeasonPoster(
            this.data.ListId, dummySeriesId, dummyPeriodId, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);
    }

    [Fact(DisplayName = "RemoveSeasonPoster should throw if the period isn't found")]
    public async Task RemoveSeasonPosterPeriodNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var dummyPeriodId = Id.Create<Period>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<PeriodNotFoundException>(() => seriesService.RemoveSeasonPoster(
            this.data.ListId, series.Id, dummyPeriodId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyPeriodId, exception.PeriodId);
    }

    [Fact(DisplayName = "RemoveSpecialEpisodePoster should remove the special episode poster")]
    public async Task RemoveSpecialEpisodePoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await this.data.CreateSpecialEpisodePoster(episode, dbContext);

        // Act

        await seriesService.RemoveSpecialEpisodePoster(
            this.data.ListId, series.Id, episode.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.SpecialEpisodePosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveSpecialEpisodePoster should not throw if poster isn't found")]
    public async Task RemoveSpecialEpisodePosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();

        // Act + Assert

        var exception = await Record.ExceptionAsync(() => seriesService.RemoveSpecialEpisodePoster(
            this.data.ListId, series.Id, episode.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveSpecialEpisodePoster should throw if the series isn't found")]
    public async Task RemoveSpecialEpisodePosterSeriesNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SeriesNotFoundException>(() =>
            seriesService.RemoveSpecialEpisodePoster(
                this.data.ListId, dummySeriesId, dummyEpisodeId, TestContext.Current.CancellationToken));

        Assert.Equal(dummySeriesId, exception.SeriesId);
    }

    [Fact(DisplayName = "RemoveSpecialEpisodePoster should throw if the special episode isn't found")]
    public async Task RemoveSpecialEpisodePosterPeriodNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var seriesService = this.CreateSeriesService(dbContext);

        var series = await this.data.CreateSeries(dbContext);
        var dummyEpisodeId = Id.Create<SpecialEpisode>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<SpecialEpisodeNotFoundException>(() =>
            seriesService.RemoveSpecialEpisodePoster(
                this.data.ListId, series.Id, dummyEpisodeId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyEpisodeId, exception.EpisodeId);
    }

    private SeriesService CreateSeriesService(CineasteDbContext dbContext) =>
        new(dbContext, this.data.PosterProvider, this.data.PosterUrlProvider, this.logger);
}
