namespace Cineaste.Server.Services;

using Cineaste.Core;
using Cineaste.Core.Domain;
using Cineaste.Persistence;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.Franchise;
using Cineaste.Shared.Models.Shared;
using Cineaste.Shared.Validation;

using Microsoft.Extensions.Logging;

public sealed class FranchiseServiceTests : ServiceTestsBase
{
    private readonly ILogger<FranchiseService> logger;

    public FranchiseServiceTests(ITestOutputHelper output)
        : base(output)
    {
        this.logger = XUnitLogger.CreateLogger<FranchiseService>(output);
    }

    [Fact(DisplayName = "GetFranchise should return the correct franchise")]
    public async Task GetFranchiseShouldReturnCorrectFranchise()
    {
        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(this.CreateMovie(this.List));
        franchise.AddSeries(this.CreateSeries(this.List));

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync();

        var model = await franchiseService.GetFranchise(franchise.Id);

        AssertTitles(franchise, model);

        Assert.Equal(franchise.Id.Value, model.Id);
        Assert.Equal(franchise.ShowTitles, model.ShowTitles);
        Assert.Equal(franchise.IsLooselyConnected, model.IsLooselyConnected);
        Assert.Equal(franchise.ContinueNumbering, model.ContinueNumbering);

        Assert.Equal(franchise.Children.Count, model.Items.Count);

        foreach (var (item, itemModel) in franchise.Children.OrderBy(item => item.SequenceNumber)
            .Zip(model.Items.OrderBy(item => item.SequenceNumber)))
        {
            Assert.Equal(item.Title.Name, itemModel.Title);

            Assert.Equal(
                item.Select(movie => movie.Year, series => series.StartYear, franchise => franchise.StartYear ?? 0),
                itemModel.StartYear);

            Assert.Equal(
                item.Select(movie => movie.Year, series => series.EndYear, franchise => franchise.EndYear ?? 0),
                itemModel.EndYear);

            Assert.Equal(
                item.Select(
                    movie => FranchiseItemType.Movie,
                    series => FranchiseItemType.Series,
                    franchise => FranchiseItemType.Franchise),
                itemModel.Type);
        }
    }

    [Fact(DisplayName = "GetFranchise should throw if franchise isn't found")]
    public async Task GetFranchiseShouldThrowIfNotFound()
    {
        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var dummyId = Id.CreateNew<Franchise>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => franchiseService.GetFranchise(dummyId));

        Assert.Equal("NotFound.Franchise", exception.MessageCode);
        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateFranchise should put it into the database")]
    public async Task CreateFranchiseShouldPutItIntoDb()
    {
        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var request = this.CreateFranchiseRequest(movie.Id);

        var model = await franchiseService.CreateFranchise(request.Validated());

        var franchise = dbContext.Franchises.Find(Id.Create<Franchise>(model.Id));

        Assert.NotNull(franchise);

        AssertTitles(request, franchise);

        Assert.Equal(request.ShowTitles, franchise.ShowTitles);
        Assert.Equal(request.IsLooselyConnected, franchise.IsLooselyConnected);
        Assert.Equal(request.ContinueNumbering, franchise.ContinueNumbering);

        foreach (var (itemRequest, item) in request.Items.OrderBy(item => item.SequenceNumber)
            .Zip(franchise.Children.OrderBy(item => item.SequenceNumber)))
        {
            Assert.Equal(itemRequest.SequenceNumber, item.SequenceNumber);
            Assert.Equal(itemRequest.ShouldDisplayNumber, item.ShouldDisplayNumber);

            item.Do(
                movie => Assert.Equal(FranchiseItemType.Movie, itemRequest.Type),
                series => Assert.Equal(FranchiseItemType.Series, itemRequest.Type),
                franchise => Assert.Equal(FranchiseItemType.Franchise, itemRequest.Type));
        }
    }

    [Fact(DisplayName = "CreateFranchise should return a correct model")]
    public async Task CreateFranchiseShouldReturnCorrectModel()
    {
        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var request = this.CreateFranchiseRequest(movie.Id);

        var model = await franchiseService.CreateFranchise(request.Validated());

        AssertTitles(request, model);

        Assert.Equal(request.ShowTitles, model.ShowTitles);
        Assert.Equal(request.IsLooselyConnected, model.IsLooselyConnected);
        Assert.Equal(request.ContinueNumbering, model.ContinueNumbering);

        foreach (var (itemRequest, itemModel) in request.Items.OrderBy(item => item.SequenceNumber)
            .Zip(model.Items.OrderBy(item => item.SequenceNumber)))
        {
            Assert.Equal(itemRequest.SequenceNumber, itemModel.SequenceNumber);
            Assert.Equal(itemRequest.ShouldDisplayNumber, itemModel.ShouldDisplayNumber);
            Assert.Equal(itemRequest.Type, itemModel.Type);
        }
    }

    [Fact(DisplayName = "CreateFranchise should throw if the list is not found")]
    public async Task CreateFranchiseShouldThrowIfListNotFound()
    {
        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var dummyListId = Id.CreateNew<CineasteList>();
        var request = this.CreateFranchiseRequest(movie.Id) with { ListId = dummyListId.Value };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.CreateFranchise(request.Validated()));

        Assert.Equal("NotFound.List", exception.MessageCode);
        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyListId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateFranchise should throw if a franchise item is not found")]
    public async Task CreateFranchiseShouldThrowIfItemNotFound()
    {
        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var dummyItemId = Guid.NewGuid();
        var request = this.CreateFranchiseRequest(movie.Id) with { Items =
            ImmutableList.Create(new FranchiseItemRequest(dummyItemId, FranchiseItemType.Movie, 1, true)).AsValue()
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.CreateFranchise(request.Validated()));

        Assert.Equal("NotFound.FranchiseItems", exception.MessageCode);
        Assert.Equal("Resource.FranchiseItems", exception.Resource);
        Assert.Equal(ImmutableList.Create(Id.Create<Movie>(dummyItemId)), exception.Properties["movieIds"]);
        Assert.Equal(ImmutableList.Create<Guid>(), exception.Properties["seriesIds"]);
        Assert.Equal(ImmutableList.Create<Guid>(), exception.Properties["franchiseIds"]);
    }

    private async Task<Movie> CreateAndSaveMovie(CineasteDbContext dbContext)
    {
        var movie = this.CreateMovie(this.List);
        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync();

        return movie;
    }

    private FranchiseRequest CreateFranchiseRequest(Id<Movie> movieId) =>
        new(
            this.List.Id.Value,
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new FranchiseItemRequest(movieId.Value, FranchiseItemType.Movie, 1, true)).AsValue(),
            false,
            false,
            false);
}
