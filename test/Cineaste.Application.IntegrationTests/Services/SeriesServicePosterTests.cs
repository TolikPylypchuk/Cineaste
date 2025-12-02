using System.Security.Cryptography;

namespace Cineaste.Application.Services;

public sealed class SeriesServicePosterTests(DataFixture data, ITestOutputHelper output)
{
    private readonly ILogger<SeriesService> logger = XUnitLogger.Create<SeriesService>(output);

    [Fact(DisplayName = "GetSeriesPoster should get the series poster")]
    public async Task GetSeriesPoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var poster = await data.CreateSeriesPoster(series, dbContext);

        // Act

        var posterContent = await seriesService.GetSeriesPoster(
            data.ListId, series.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetSeriesPoster should throw if series isn't found")]
    public async Task GetSeriesPosterSeriesNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Series>();

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.GetSeriesPoster(data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "GetSeriesPoster should throw if poster isn't found")]
    public async Task GetSeriesPosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);

        // Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.GetSeriesPoster(data.ListId, series.Id, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Poster", exception.Resource);
        Assert.Equal(series.Id, exception.Properties["seriesId"]);
    }

    [Fact(DisplayName = "GetSeasonPoster should get the season poster")]
    public async Task GetSeasonPoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await data.CreateSeasonPoster(period, dbContext);

        // Act

        var posterContent = await seriesService.GetSeasonPoster(
            data.ListId, series.Id, period.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetSeasonPoster should throw if series isn't found")]
    public async Task GetSeasonPosterSeriesNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.GetSeasonPoster(
                data.ListId, dummySeriesId, dummyPeriodId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "GetSeasonPoster should throw if period isn't found")]
    public async Task GetSeasonPosterPeriodNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var dummyId = Id.Create<Period>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.GetSeasonPoster(data.ListId, series.Id, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Period", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "GetSeasonPoster should throw if poster isn't found")]
    public async Task GetSeasonPosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.GetSeasonPoster(data.ListId, series.Id, period.Id, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Poster", exception.Resource);
        Assert.Equal(period.Id, exception.Properties["periodId"]);
    }

    [Fact(DisplayName = "GetSpecialEpisodePoster should get the special episode poster")]
    public async Task GetSpecialEpisodePoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await data.CreateSpecialEpisodePoster(episode, dbContext);

        // Act

        var posterContent = await seriesService.GetSpecialEpisodePoster(
            data.ListId, series.Id, episode.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetSpecialEpisodePoster should throw if series isn't found")]
    public async Task GetSpecialEpisodePosterSeriesNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.GetSpecialEpisodePoster(
            data.ListId, dummySeriesId, dummyEpisodeId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "GetSpecialEpisodePoster should throw if episode isn't found")]
    public async Task GetSpecialEpisodePosterEpisodeNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var dummyId = Id.Create<SpecialEpisode>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.GetSpecialEpisodePoster(
                data.ListId, series.Id, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.SpecialEpisode", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "GetSpecialEpisodePoster should throw if poster isn't found")]
    public async Task GetSpecialEpisodePosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.GetSpecialEpisodePoster(
                data.ListId, series.Id, episode.Id, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Poster", exception.Resource);
        Assert.Equal(episode.Id, exception.Properties["episodeId"]);
    }

    [Fact(DisplayName = "SetSeriesPoster should set the series poster from a stream")]
    public async Task SetSeriesPosterStream()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var poster = await data.CreateSeriesPoster(series, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await seriesService.SetSeriesPoster(
            data.ListId, series.Id, posterData, TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Series>();
        var posterData = data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.SetSeriesPoster(data.ListId, dummyId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "SetSeriesPoster should set the series poster from a URL")]
    public async Task SetSeriesPosterUrl()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var poster = await data.CreateSeriesPoster(series, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterUrlRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSeriesPoster(
            data.ListId, series.Id, request, TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Series>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.SetSeriesPoster(data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSeriesPoster should set the series poster from IMDb media")]
    public async Task SetSeriesPosterImdbSeriesMedia()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var poster = await data.CreateSeriesPoster(series, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterImdbMediaRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSeriesPoster(
            data.ListId, series.Id, request, TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Series>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.SetSeriesPoster(data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSeasonPoster should set the season poster from a stream")]
    public async Task SetSeasonPosterStream()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await data.CreateSeasonPoster(period, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await seriesService.SetSeasonPoster(
            data.ListId, series.Id, period.Id, posterData, TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();
        var posterData = data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSeasonPoster(
            data.ListId, dummySeriesId, dummyPeriodId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "SetSeasonPoster from stream should throw if the period isn't found")]
    public async Task SetSeasonPosterStreamPeriodNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var dummyPeriodId = Id.Create<Period>();
        var posterData = data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSeasonPoster(
            data.ListId, series.Id, dummyPeriodId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Period", exception.Resource);
        Assert.Equal(dummyPeriodId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "SetSeasonPoster should set the season poster from a URL")]
    public async Task SetSeasonPosterUrl()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await data.CreateSeasonPoster(period, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterUrlRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSeasonPoster(
            data.ListId, series.Id, period.Id, request, TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSeasonPoster(
            data.ListId, dummySeriesId, dummyPeriodId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSeasonPoster from a URL should throw if the period isn't found")]
    public async Task SetSeasonPosterUrlPeriodNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var dummyPeriodId = Id.Create<Period>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSeasonPoster(
            data.ListId, series.Id, dummyPeriodId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Period", exception.Resource);
        Assert.Equal(dummyPeriodId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSeasonPoster should set the season poster from IMDb media")]
    public async Task SetSeasonPosterImdbSeriesMedia()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await data.CreateSeasonPoster(period, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterImdbMediaRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSeasonPoster(
            data.ListId, series.Id, period.Id, request, TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSeasonPoster(
            data.ListId, dummySeriesId, dummyPeriodId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster should set the special episode poster from a stream")]
    public async Task SetSpecialEpisodePosterStream()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await data.CreateSpecialEpisodePoster(episode, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await seriesService.SetSpecialEpisodePoster(
            data.ListId, series.Id, episode.Id, posterData, TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSpecialEpisodePoster(
            data.ListId, dummySeriesId, dummyEpisodeId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster from stream should throw if the special episode isn't found")]
    public async Task SetSpecialEpisodePosterStreamPeriodNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSpecialEpisodePoster(
            data.ListId, series.Id, dummyEpisodeId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.SpecialEpisode", exception.Resource);
        Assert.Equal(dummyEpisodeId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster should set the special episode poster from a URL")]
    public async Task SetSpecialEpisodePosterUrl()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await data.CreateSpecialEpisodePoster(episode, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterUrlRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSpecialEpisodePoster(
            data.ListId, series.Id, episode.Id, request, TestContext.Current.CancellationToken);

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

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSpecialEpisodePoster(
            data.ListId, dummySeriesId, dummyEpisodeId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster from a URL should throw if the special episode isn't found")]
    public async Task SetSpecialEpisodePosterUrlPeriodNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSpecialEpisodePoster(
            data.ListId, series.Id, dummyEpisodeId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.SpecialEpisode", exception.Resource);
        Assert.Equal(dummyEpisodeId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster should set the special episode poster from IMDb media")]
    public async Task SetSpecialEpisodePosterImdbSeriesMedia()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await data.CreateSpecialEpisodePoster(episode, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterImdbMediaRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await seriesService.SetSpecialEpisodePoster(
            data.ListId, series.Id, episode.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.SpecialEpisodePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.SpecialEpisodePosters.FirstOrDefault(p => p.SpecialEpisode == episode);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetSpecialEpisodePoster from IMDb media should throw if the special episode isn't found")]
    public async Task SetSpecialEpisodePosterImdbMediaSeriesNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.SetSpecialEpisodePoster(
            data.ListId, dummySeriesId, dummyEpisodeId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "RemoveSeriesPoster should remove the series poster")]
    public async Task RemoveSeriesPoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var poster = await data.CreateSeriesPoster(series, dbContext);

        // Act

        await seriesService.RemoveSeriesPoster(data.ListId, series.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.SeriesPosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveSeriesPoster should not throw if poster isn't found")]
    public async Task RemoveSeriesPosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);

        // Act + Assert

        var exception = await Record.ExceptionAsync(() =>
            seriesService.RemoveSeriesPoster(data.ListId, series.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveSeriesPoster should throw if the series isn't found")]
    public async Task RemoveSeriesPosterSeriesNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Series>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            seriesService.RemoveSeriesPoster(data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "RemoveSeasonPoster should remove the series poster")]
    public async Task RemoveSeasonPoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();
        var poster = await data.CreateSeasonPoster(period, dbContext);

        // Act

        await seriesService.RemoveSeasonPoster(
            data.ListId, series.Id, period.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.SeasonPosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveSeasonPoster should not throw if poster isn't found")]
    public async Task RemoveSeasonPosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var period = series.Seasons.First().Periods.First();

        // Act + Assert

        var exception = await Record.ExceptionAsync(() => seriesService.RemoveSeasonPoster(
            data.ListId, series.Id, period.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveSeasonPoster should throw if the series isn't found")]
    public async Task RemoveSeasonPosterSeriesNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyPeriodId = Id.Create<Period>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.RemoveSeasonPoster(
            data.ListId, dummySeriesId, dummyPeriodId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "RemoveSeasonPoster should throw if the period isn't found")]
    public async Task RemoveSeasonPosterPeriodNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var dummyPeriodId = Id.Create<Period>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.RemoveSeasonPoster(
            data.ListId, series.Id, dummyPeriodId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Period", exception.Resource);
        Assert.Equal(dummyPeriodId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "RemoveSpecialEpisodePoster should remove the special episode poster")]
    public async Task RemoveSpecialEpisodePoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();
        var poster = await data.CreateSpecialEpisodePoster(episode, dbContext);

        // Act

        await seriesService.RemoveSpecialEpisodePoster(
            data.ListId, series.Id, episode.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.SpecialEpisodePosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveSpecialEpisodePoster should not throw if poster isn't found")]
    public async Task RemoveSpecialEpisodePosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var episode = series.SpecialEpisodes.First();

        // Act + Assert

        var exception = await Record.ExceptionAsync(() => seriesService.RemoveSpecialEpisodePoster(
            data.ListId, series.Id, episode.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveSpecialEpisodePoster should throw if the series isn't found")]
    public async Task RemoveSpecialEpisodePosterSeriesNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var dummySeriesId = Id.Create<Series>();
        var dummyEpisodeId = Id.Create<SpecialEpisode>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.RemoveSpecialEpisodePoster(
            data.ListId, dummySeriesId, dummyEpisodeId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Series", exception.Resource);
        Assert.Equal(dummySeriesId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "RemoveSpecialEpisodePoster should throw if the special episode isn't found")]
    public async Task RemoveSpecialEpisodePosterPeriodNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var seriesService = new SeriesService(dbContext, data.PosterProvider, this.logger);

        var series = await data.CreateSeries(dbContext);
        var dummyEpisodeId = Id.Create<SpecialEpisode>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => seriesService.RemoveSpecialEpisodePoster(
            data.ListId, series.Id, dummyEpisodeId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.SpecialEpisode", exception.Resource);
        Assert.Equal(dummyEpisodeId, exception.Properties["id"]);
    }
}
