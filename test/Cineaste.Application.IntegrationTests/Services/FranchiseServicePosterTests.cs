using System.Security.Cryptography;

namespace Cineaste.Application.Services;

public sealed class FranchiseServicePosterTests(DbFixture dbFixture, ITestOutputHelper output)
    : TestClassBase(dbFixture)
{
    private readonly ILogger<FranchiseService> logger = XUnitLogger.Create<FranchiseService>(output);

    [Fact(DisplayName = "GetFranchisePoster should get the franchise poster")]
    public async Task GetFranchisePoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var franchise = await this.data.CreateFranchise(dbContext);
        var poster = await this.data.CreateFranchisePoster(franchise, dbContext);

        // Act

        var posterContent = await franchiseService.GetFranchisePoster(
            this.data.ListId, franchise.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(poster.Data, posterContent.Data);
        Assert.Equal(poster.ContentType, posterContent.Type);
    }

    [Fact(DisplayName = "GetFranchisePoster should throw if franchise isn't found")]
    public async Task GetFranchisePosterFranchiseNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<FranchiseNotFoundException>(() =>
            franchiseService.GetFranchisePoster(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.FranchiseId);
    }

    [Fact(DisplayName = "GetFranchisePoster should throw if poster isn't found")]
    public async Task GetFranchisePosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var franchise = await this.data.CreateFranchise(dbContext);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<FranchisePosterNotFoundException>(() =>
            franchiseService.GetFranchisePoster(this.data.ListId, franchise.Id, TestContext.Current.CancellationToken));

        Assert.Equal(franchise.Id, exception.FranchiseId);
    }

    [Fact(DisplayName = "SetFranchisePoster should set the franchise poster from a stream")]
    public async Task SetFranchisePosterStream()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var franchise = await this.data.CreateFranchise(dbContext);
        var poster = await this.data.CreateFranchisePoster(franchise, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        // Act

        var posterHash = await franchiseService.SetFranchisePoster(
            this.data.ListId, franchise.Id, posterData, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.FranchisePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.FranchisePosters.FirstOrDefault(p => p.Franchise == franchise);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetFranchisePoster from stream should throw if the franchise isn't found")]
    public async Task SetFranchisePosterStreamFranchiseNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var dummyId = Id.Create<Franchise>();
        var posterData = this.data.CreatePosterContent();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<FranchiseNotFoundException>(() =>
            franchiseService.SetFranchisePoster(
                this.data.ListId, dummyId, posterData, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.FranchiseId);
    }

    [Fact(DisplayName = "SetFranchisePoster should set the franchise poster from a URL")]
    public async Task SetFranchisePosterUrl()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var franchise = await this.data.CreateFranchise(dbContext);
        var poster = await this.data.CreateFranchisePoster(franchise, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterUrlRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await franchiseService.SetFranchisePoster(
            this.data.ListId, franchise.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.FranchisePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.FranchisePosters.FirstOrDefault(p => p.Franchise == franchise);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetFranchisePoster from a URL should throw if the franchise isn't found")]
    public async Task SetFranchisePosterUrlFranchiseNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var dummyId = Id.Create<Franchise>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterUrlRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<FranchiseNotFoundException>(() =>
            franchiseService.SetFranchisePoster(this.data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.FranchiseId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "SetFranchisePoster should set the franchise poster from IMDb media")]
    public async Task SetFranchisePosterImdbMovieMedia()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var franchise = await this.data.CreateFranchise(dbContext);
        var poster = await this.data.CreateFranchisePoster(franchise, dbContext);

        var posterData = this.data.CreatePosterContent();
        var posterContent = await posterData.ReadDataAsync(TestContext.Current.CancellationToken);

        var request = this.data.CreatePosterImdbMediaRequest();

        this.data.PosterProvider
            .FetchPoster(request, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(posterData));

        // Act

        var posterHash = await franchiseService.SetFranchisePoster(
            this.data.ListId, franchise.Id, request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(SHA256.HashData(posterContent.Data), Convert.FromHexString(posterHash.Value));

        Assert.False(dbContext.FranchisePosters.Any(p => p.Id == poster.Id));

        var dbPoster = dbContext.FranchisePosters.FirstOrDefault(p => p.Franchise == franchise);

        Assert.NotNull(dbPoster);
        Assert.Equal(posterContent.Data, dbPoster.Data);
        Assert.Equal(posterContent.Type, dbPoster.ContentType);
    }

    [Fact(DisplayName = "SetFranchisePoster from IMDb media should throw if the franchise isn't found")]
    public async Task SetFranchisePosterImdbMediaFranchiseNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var dummyId = Id.Create<Franchise>();
        var posterData = this.data.CreatePosterContent();

        var request = this.data.CreatePosterImdbMediaRequest();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<FranchiseNotFoundException>(() =>
            franchiseService.SetFranchisePoster(this.data.ListId, dummyId, request, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.FranchiseId);

        await this.data.PosterProvider
            .DidNotReceive()
            .FetchPoster(request, TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "RemoveFranchisePoster should remove the franchise poster")]
    public async Task RemoveFranchisePoster()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var franchise = await this.data.CreateFranchise(dbContext);
        var poster = await this.data.CreateFranchisePoster(franchise, dbContext);

        // Act

        await franchiseService.RemoveFranchisePoster(this.data.ListId, franchise.Id, TestContext.Current.CancellationToken);

        // Assert

        Assert.False(dbContext.FranchisePosters.Any(p => p.Id == poster.Id));
    }

    [Fact(DisplayName = "RemoveFranchisePoster should not throw if poster isn't found")]
    public async Task RemoveFranchisePosterNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var franchise = await this.data.CreateFranchise(dbContext);

        // Act + Assert

        var exception = await Record.ExceptionAsync(() =>
            franchiseService.RemoveFranchisePoster(this.data.ListId, franchise.Id, TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    [Fact(DisplayName = "RemoveFranchisePoster should throw if the franchise isn't found")]
    public async Task RemoveFranchisePosterFranchiseNotFound()
    {
        // Arrange

        var dbContext = this.data.CreateDbContext();
        var franchiseService = this.CreateFranchiseService(dbContext);

        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<FranchiseNotFoundException>(() =>
            franchiseService.RemoveFranchisePoster(this.data.ListId, dummyId, TestContext.Current.CancellationToken));

        Assert.Equal(dummyId, exception.FranchiseId);
    }

    private FranchiseService CreateFranchiseService(CineasteDbContext dbContext) =>
        new(dbContext, this.data.PosterProvider, this.data.PosterUrlProvider, this.logger);
}
