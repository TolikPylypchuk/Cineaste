using System.Security.Cryptography;

namespace Cineaste.Services;

public sealed class FranchiseServicePosterTests(DataFixture data, ITestOutputHelper output)
{
    private readonly ILogger<FranchiseService> logger = XUnitLogger.Create<FranchiseService>(output);

    [Fact(DisplayName = "GetFranchisePoster should get the franchise poster")]
    public async Task GetFranchisePoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var franchise = await data.CreateFranchise(dbContext);
        var poster = await data.CreateFranchisePoster(franchise, dbContext);

        // Act

        var posterContent = await franchiseService.GetFranchisePoster(
            data.ListId, franchise.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetFranchisePoster should throw if franchise isn't found")]
    public async Task GetFranchisePosterFranchiseNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.GetFranchisePoster(data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "GetFranchisePoster should throw if poster isn't found")]
    public async Task GetFranchisePosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var franchise = await data.CreateFranchise(dbContext);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.GetFranchisePoster(data.ListId, franchise.Id, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Poster", exception.Resource);
        Assert.Equal(franchise.Id, exception.Properties["franchiseId"]);
    }

    [Fact(DisplayName = "SetFranchisePoster should set the franchise poster from a stream")]
    public async Task SetFranchisePosterStream()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var franchise = await data.CreateFranchise(dbContext);
        var poster = await data.CreateFranchisePoster(franchise, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await franchiseService.SetFranchisePoster(
            data.ListId, franchise.Id, posterData, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.FranchisePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.FranchisePosters.FirstOrDefault(p => p.Franchise == franchise);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetFranchisePoster from stream should throw if the franchise isn't found")]
    public async Task SetFranchisePosterStreamMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Franchise>();
        var posterData = data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.SetFranchisePoster(
                data.ListId, dummyId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "SetFranchisePoster should set the franchise poster from a URL")]
    public async Task SetFranchisePosterUrl()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var franchise = await data.CreateFranchise(dbContext);
        var poster = await data.CreateFranchisePoster(franchise, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterUrlRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await franchiseService.SetFranchisePoster(
            data.ListId, franchise.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.FranchisePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.FranchisePosters.FirstOrDefault(p => p.Franchise == franchise);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetFranchisePoster from a URL should throw if the franchise isn't found")]
    public async Task SetFranchisePosterUrlMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Franchise>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.SetFranchisePoster(data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetFranchisePoster should set the franchise poster from IMDb media")]
    public async Task SetFranchisePosterImdbMovieMedia()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var franchise = await data.CreateFranchise(dbContext);
        var poster = await data.CreateFranchisePoster(franchise, dbContext);

        var posterData = data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = data.CreatePosterImdbMediaRequest();

        data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await franchiseService.SetFranchisePoster(
            data.ListId, franchise.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.FranchisePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.FranchisePosters.FirstOrDefault(p => p.Franchise == franchise);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetFranchisePoster from IMDb media should throw if the franchise isn't found")]
    public async Task SetFranchisePosterImdbMediaMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Franchise>();
        var posterData = data.CreatePosterContent();

        var request = data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.SetFranchisePoster(data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);

        await data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "RemoveFranchisePoster should remove the franchise poster")]
    public async Task RemoveFranchisePoster()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var franchise = await data.CreateFranchise(dbContext);
        var poster = await data.CreateFranchisePoster(franchise, dbContext);

        // Act

        await franchiseService.RemoveFranchisePoster(data.ListId, franchise.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.FranchisePosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveFranchisePoster should not throw if poster isn't found")]
    public async Task RemoveFranchisePosterNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var franchise = await data.CreateFranchise(dbContext);

        // Act + Assert

        var exception = await Record.ExceptionAsync(() =>
            franchiseService.RemoveFranchisePoster(data.ListId, franchise.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveFranchisePoster should throw if the franchise isn't found")]
    public async Task RemoveFranchisePosterMovieNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, data.PosterProvider, this.logger);

        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.RemoveFranchisePoster(data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }
}
