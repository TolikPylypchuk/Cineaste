namespace Cineaste.Server.Services;

using Cineaste.Core;
using Cineaste.Core.Domain;
using Cineaste.Persistence;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.Franchise;
using Cineaste.Shared.Models.Shared;
using Cineaste.Shared.Validation;

using Microsoft.Extensions.Logging;

public sealed class FranchiseServiceTests(ITestOutputHelper output) : ServiceTestsBase
{
    private readonly ILogger<FranchiseService> logger = XUnitLogger.Create<FranchiseService>(output);

    [Fact(DisplayName = "GetFranchise should return the correct franchise")]
    public async Task GetFranchiseShouldReturnCorrectFranchise()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(this.CreateMovie(this.List));
        franchise.AddSeries(this.CreateSeries(this.List));

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act

        var model = await franchiseService.GetFranchise(franchise.Id);

        // Assert

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
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => franchiseService.GetFranchise(dummyId));

        Assert.Equal("NotFound.Franchise", exception.MessageCode);
        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateFranchise should put it into the database")]
    public async Task CreateFranchiseShouldPutItIntoDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var request = this.CreateFranchiseRequest(movie.Id);

        // Act

        var model = await franchiseService.CreateFranchise(request.Validated());

        // Assert

        var franchise = dbContext.Franchises.Find(Id.For<Franchise>(model.Id));

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
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var request = this.CreateFranchiseRequest(movie.Id);

        // Act

        var model = await franchiseService.CreateFranchise(request.Validated());

        // Assert

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
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var dummyListId = Id.Create<CineasteList>();
        var request = this.CreateFranchiseRequest(movie.Id) with { ListId = dummyListId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.CreateFranchise(request.Validated()));

        Assert.Equal("NotFound.List", exception.MessageCode);
        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyListId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "CreateFranchise should throw if a franchise item is not found")]
    public async Task CreateFranchiseShouldThrowIfItemNotFound()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var dummyItemId = Guid.NewGuid();
        var request = this.CreateFranchiseRequest(movie.Id) with { Items =
            ImmutableList.Create(new FranchiseItemRequest(dummyItemId, FranchiseItemType.Movie, 1, true)).AsValue()
        };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.CreateFranchise(request.Validated()));

        Assert.Equal("NotFound.FranchiseItems", exception.MessageCode);
        Assert.Equal("Resource.FranchiseItems", exception.Resource);
        Assert.Equal(ImmutableList.Create(Id.For<Movie>(dummyItemId)), exception.Properties["movieIds"]);
        Assert.Equal(ImmutableList.Create<Guid>(), exception.Properties["seriesIds"]);
        Assert.Equal(ImmutableList.Create<Guid>(), exception.Properties["franchiseIds"]);
    }

    [Fact(DisplayName = "UpdateFranchise should update it in the database")]
    public async Task UpdateFranchiseShouldUpdateItInDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie1 = await this.CreateAndSaveMovie(dbContext);
        var movie2 = await this.CreateAndSaveMovie(dbContext);
        var movie3 = await this.CreateAndSaveMovie(dbContext);

        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(movie1);
        franchise.AddMovie(movie2);

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(
            showTitles: true,
            items:
            [
                new FranchiseItemRequest(movie1.Id.Value, FranchiseItemType.Movie, 1, true),
                new FranchiseItemRequest(movie3.Id.Value, FranchiseItemType.Movie, 2, true)
            ]);

        // Act

        var model = await franchiseService.UpdateFranchise(franchise.Id, request.Validated());

        // Assert

        var dbFranchise = dbContext.Franchises.Find(Id.For<Franchise>(model.Id));

        Assert.NotNull(dbFranchise);

        AssertTitles(request, model);

        Assert.Equal(request.ShowTitles, dbFranchise.ShowTitles);
        Assert.Equal(request.IsLooselyConnected, dbFranchise.IsLooselyConnected);
        Assert.Equal(request.ContinueNumbering, dbFranchise.ContinueNumbering);

        foreach (var (itemRequest, item) in request.Items.OrderBy(item => item.SequenceNumber)
            .Zip(dbFranchise.Children.OrderBy(item => item.SequenceNumber)))
        {
            var itemType = item.Select(
                m => FranchiseItemType.Movie,
                s => FranchiseItemType.Series,
                f => FranchiseItemType.Franchise);

            Assert.Equal(itemRequest.Id, item.Select(m => m.Id.Value, s => s.Id.Value, f => f.Id.Value));
            Assert.Equal(itemRequest.Type, itemType);
            Assert.Equal(itemRequest.SequenceNumber, item.SequenceNumber);
            Assert.Equal(itemRequest.ShouldDisplayNumber, item.ShouldDisplayNumber);
        }
    }

    [Fact(DisplayName = "UpdateFranchise should ignore titles if ShowTitles = false")]
    public async Task UpdateFranchiseShouldIgnoreTitlesIfShowTitlesFalse()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(movie);

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(movie.Id, showTitles: false);

        // Act

        var model = await franchiseService.UpdateFranchise(franchise.Id, request.Validated());

        // Assert

        var dbFranchise = dbContext.Franchises.Find(Id.For<Franchise>(model.Id));

        Assert.NotNull(dbFranchise);
        Assert.Empty(dbFranchise.Titles);
    }

    [Fact(DisplayName = "UpdateFranchise should return a correct model")]
    public async Task UpdateFranchiseShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(movie);

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(movie.Id, showTitles: true);

        // Act

        var model = await franchiseService.UpdateFranchise(franchise.Id, request.Validated());

        // Assert

        AssertTitles(request, model);

        Assert.Equal(request.ShowTitles, model.ShowTitles);
        Assert.Equal(request.IsLooselyConnected, model.IsLooselyConnected);
        Assert.Equal(request.ContinueNumbering, model.ContinueNumbering);

        foreach (var (itemRequest, itemModel) in request.Items.OrderBy(item => item.SequenceNumber)
            .Zip(model.Items.OrderBy(item => item.SequenceNumber)))
        {
            Assert.Equal(itemRequest.Id, itemModel.Id);
            Assert.Equal(itemRequest.Type, itemModel.Type);
            Assert.Equal(itemRequest.SequenceNumber, itemModel.SequenceNumber);
            Assert.Equal(itemRequest.ShouldDisplayNumber, itemModel.ShouldDisplayNumber);
        }
    }

    [Fact(DisplayName = "UpdateFranchise should throw if not found")]
    public async Task UpdateFranchiseShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(movie);

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(movie.Id, showTitles: true);
        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.UpdateFranchise(dummyId, request.Validated()));

        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "UpdateFranchise should throw if movie doesn't belong to list")]
    public async Task UpdateFranchiseShouldThrowIfFranchiseDoesNotBelongToList()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(movie);

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var otherList = this.CreateList();
        dbContext.Lists.Add(otherList);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(movie.Id) with { ListId = otherList.Id.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<InvalidInputException>(() =>
            franchiseService.UpdateFranchise(franchise.Id, request.Validated()));

        Assert.Equal("BadRequest.Franchise.WrongList", exception.MessageCode);
        Assert.Equal(franchise.Id, exception.Properties["franchiseId"]);
        Assert.Equal(otherList.Id, exception.Properties["listId"]);
    }

    [Fact(DisplayName = "DeleteFranchise should remote it from the database")]
    public async Task DeleteFranchiseShouldRemoveItFromDb()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await this.CreateAndSaveMovie(dbContext);
        var franchise = this.CreateFranchise(this.List);
        franchise.AddMovie(movie);

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act

        await franchiseService.DeleteFranchise(franchise.Id);

        // Assert

        Assert.True(dbContext.Franchises.All(f => f.Id != franchise.Id));
    }

    [Fact(DisplayName = "DeleteFranchise should throw if franchise isn't found")]
    public async Task DeleteFranchiseShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => franchiseService.DeleteFranchise(dummyId));

        Assert.Equal("NotFound.Franchise", exception.MessageCode);
        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private async Task<Movie> CreateAndSaveMovie(CineasteDbContext dbContext)
    {
        var movie = this.CreateMovie(this.List);
        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync();

        return movie;
    }

    private FranchiseRequest CreateFranchiseRequest(
        Id<Movie> movieId,
        bool showTitles = false,
        bool isLooselyConnected = false,
        bool continueNumbering = false) =>
        this.CreateFranchiseRequest(
            showTitles,
            isLooselyConnected,
            continueNumbering,
            new FranchiseItemRequest(movieId.Value, FranchiseItemType.Movie, 1, true));

    private FranchiseRequest CreateFranchiseRequest(
        bool showTitles = false,
        bool isLooselyConnected = false,
        bool continueNumbering = false,
        params FranchiseItemRequest[] items) =>
        new(
            this.List.Id.Value,
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(items).AsValue(),
            showTitles,
            isLooselyConnected,
            continueNumbering);
}
