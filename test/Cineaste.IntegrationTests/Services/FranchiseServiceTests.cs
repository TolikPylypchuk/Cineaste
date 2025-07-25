using Cineaste.Shared.Models.Franchise;

namespace Cineaste.Services;

public sealed class FranchiseServiceTests(DataFixture data, ITestOutputHelper output)
{
    private readonly ILogger<FranchiseService> logger = XUnitLogger.Create<FranchiseService>(output);

    [Fact(DisplayName = "GetFranchise should return the correct franchise")]
    public async Task GetFranchiseShouldReturnCorrectFranchise()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var list = await data.GetList(dbContext);

        var franchise = await data.CreateFranchise(dbContext);

        franchise.AddMovie(await data.CreateMovie(dbContext), true);
        franchise.AddSeries(await data.CreateSeries(dbContext), true);

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

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => franchiseService.GetFranchise(dummyId));

        Assert.Equal("NotFound.Franchise", exception.MessageCode);
        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "AddFranchise should put it into the database")]
    public async Task AddFranchiseShouldPutItIntoDb()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var request = this.CreateFranchiseRequest(movie.Id, movie.Kind.Id.Value, FranchiseKindSource.Movie);

        // Act

        var model = await franchiseService.AddFranchise(request.Validated());

        // Assert

        var franchise = dbContext.Franchises.Find(Id.For<Franchise>(model.Id));

        Assert.NotNull(franchise);

        AssertTitles(request, franchise);

        var actualKindId = franchise.KindSource == FranchiseKindSource.Movie
            ? franchise.MovieKind.Id.Value
            : franchise.SeriesKind.Id.Value;

        Assert.Equal(request.KindId, actualKindId);
        Assert.Equal(request.KindSource, franchise.KindSource);
        Assert.Equal(request.ShowTitles, franchise.ShowTitles);
        Assert.Equal(request.IsLooselyConnected, franchise.IsLooselyConnected);
        Assert.Equal(request.ContinueNumbering, franchise.ContinueNumbering);

        foreach (var (itemRequest, item) in request.Items.OrderBy(item => item.SequenceNumber)
            .Zip(franchise.Children.OrderBy(item => item.SequenceNumber)))
        {
            Assert.Equal(itemRequest.SequenceNumber, item.SequenceNumber);
            Assert.Equal(itemRequest.ShouldDisplayNumber, item.DisplayNumber is not null);

            item.Do(
                movie => Assert.Equal(FranchiseItemType.Movie, itemRequest.Type),
                series => Assert.Equal(FranchiseItemType.Series, itemRequest.Type),
                franchise => Assert.Equal(FranchiseItemType.Franchise, itemRequest.Type));
        }
    }

    [Fact(DisplayName = "AddFranchise should return a correct model")]
    public async Task AddFranchiseShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var request = this.CreateFranchiseRequest(movie.Id, movie.Kind.Id.Value, FranchiseKindSource.Movie);

        // Act

        var model = await franchiseService.AddFranchise(request.Validated());

        // Assert

        AssertTitles(request, model);

        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.KindSource, model.KindSource);
        Assert.Equal(request.ShowTitles, model.ShowTitles);
        Assert.Equal(request.IsLooselyConnected, model.IsLooselyConnected);
        Assert.Equal(request.ContinueNumbering, model.ContinueNumbering);

        foreach (var (itemRequest, itemModel) in request.Items.OrderBy(item => item.SequenceNumber)
            .Zip(model.Items.OrderBy(item => item.SequenceNumber)))
        {
            Assert.Equal(itemRequest.SequenceNumber, itemModel.SequenceNumber);
            Assert.Equal(itemRequest.ShouldDisplayNumber, itemModel.DisplayNumber is not null);
            Assert.Equal(itemRequest.Type, itemModel.Type);
        }
    }

    [Fact(DisplayName = "AddFranchise should throw if the list is not found")]
    public async Task AddFranchiseShouldThrowIfListNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var dummyListId = Id.Create<CineasteList>();
        var request = this.CreateFranchiseRequest(movie.Id, movie.Kind.Id.Value, FranchiseKindSource.Movie)
            with { ListId = dummyListId.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.AddFranchise(request.Validated()));

        Assert.Equal("NotFound.List", exception.MessageCode);
        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyListId, exception.Properties["id"]);
    }

    [Fact(DisplayName = "AddFranchise should throw if a franchise item is not found")]
    public async Task AddFranchiseShouldThrowIfItemNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie = await data.CreateMovie(dbContext);
        var dummyItemId = Guid.CreateVersion7();
        var request = this.CreateFranchiseRequest(movie.Id, movie.Kind.Id.Value, FranchiseKindSource.Movie) with
        {
            Items = ImmutableList.Create(new FranchiseItemRequest(dummyItemId, FranchiseItemType.Movie, 1, true))
                .AsValue()
        };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            franchiseService.AddFranchise(request.Validated()));

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

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var movie1 = await data.CreateMovie(dbContext);
        var movie2 = await data.CreateMovie(dbContext);
        var movie3 = await data.CreateMovie(dbContext);

        var list = await data.GetList(dbContext);

        var franchise = await data.CreateFranchise(dbContext);
        franchise.AddMovie(movie1, true);
        franchise.AddMovie(movie2, true);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(
            list.MovieKinds.First().Id.Value,
            FranchiseKindSource.Movie,
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

        var actualKindId = dbFranchise.KindSource == FranchiseKindSource.Movie
            ? dbFranchise.MovieKind.Id.Value
            : dbFranchise.SeriesKind.Id.Value;

        Assert.Equal(request.KindId, actualKindId);
        Assert.Equal(request.KindSource, dbFranchise.KindSource);
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
            Assert.Equal(itemRequest.ShouldDisplayNumber, item.DisplayNumber is not null);
        }
    }

    [Fact(DisplayName = "UpdateFranchise should return a correct model")]
    public async Task UpdateFranchiseShouldReturnCorrectModel()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var list = await data.GetList(dbContext);

        var movie = await data.CreateMovie(dbContext);
        var franchise = await data.CreateFranchise(dbContext);
        franchise.AddMovie(movie, true);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(
            movie.Id, movie.Kind.Id.Value, FranchiseKindSource.Movie, showTitles: true);

        // Act

        var model = await franchiseService.UpdateFranchise(franchise.Id, request.Validated());

        // Assert

        AssertTitles(request, model);

        Assert.Equal(request.KindId, model.Kind.Id);
        Assert.Equal(request.KindSource, model.KindSource);
        Assert.Equal(request.ShowTitles, model.ShowTitles);
        Assert.Equal(request.ShowTitles, model.ShowTitles);
        Assert.Equal(request.IsLooselyConnected, model.IsLooselyConnected);
        Assert.Equal(request.ContinueNumbering, model.ContinueNumbering);

        foreach (var (itemRequest, itemModel) in request.Items.OrderBy(item => item.SequenceNumber)
            .Zip(model.Items.OrderBy(item => item.SequenceNumber)))
        {
            Assert.Equal(itemRequest.Id, itemModel.Id);
            Assert.Equal(itemRequest.Type, itemModel.Type);
            Assert.Equal(itemRequest.SequenceNumber, itemModel.SequenceNumber);
            Assert.Equal(itemRequest.ShouldDisplayNumber, itemModel.DisplayNumber is not null);
        }
    }

    [Fact(DisplayName = "UpdateFranchise should throw if not found")]
    public async Task UpdateFranchiseShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var list = await data.GetList(dbContext);

        var movie = await data.CreateMovie(dbContext);
        var franchise = await data.CreateFranchise(dbContext);
        franchise.AddMovie(movie, true);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(
            movie.Id, list.MovieKinds.First().Id.Value, FranchiseKindSource.Movie, showTitles: true);

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

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var list = await data.GetList(dbContext);

        var movie = await data.CreateMovie(dbContext);
        var franchise = await data.CreateFranchise(dbContext);
        franchise.AddMovie(movie, true);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var otherList = data.CreateList();
        dbContext.Lists.Add(otherList);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = this.CreateFranchiseRequest(movie.Id, movie.Kind.Id.Value, FranchiseKindSource.Movie)
            with { ListId = otherList.Id.Value };

        // Act + Assert

        var exception = await Assert.ThrowsAsync<InvalidInputException>(() =>
            franchiseService.UpdateFranchise(franchise.Id, request.Validated()));

        Assert.Equal("BadRequest.Franchise.WrongList", exception.MessageCode);
        Assert.Equal(franchise.Id, exception.Properties["franchiseId"]);
        Assert.Equal(otherList.Id, exception.Properties["listId"]);

        // Clean up

        dbContext.Lists.Remove(otherList);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact(DisplayName = "RemoveFranchise should remote it from the database")]
    public async Task RemoveFranchiseShouldRemoveItFromDb()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var list = await data.GetList(dbContext);

        var movie = await data.CreateMovie(dbContext);
        var franchise = await data.CreateFranchise(dbContext);
        franchise.AddMovie(movie, true);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act

        await franchiseService.RemoveFranchise(franchise.Id);

        // Assert

        Assert.True(dbContext.Franchises.All(f => f.Id != franchise.Id));
    }

    [Fact(DisplayName = "RemoveFranchise should throw if franchise isn't found")]
    public async Task RemoveFranchiseShouldThrowIfNotFound()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        var dummyId = Id.Create<Franchise>();

        // Act + Assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => franchiseService.RemoveFranchise(dummyId));

        Assert.Equal("NotFound.Franchise", exception.MessageCode);
        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private FranchiseRequest CreateFranchiseRequest(
        Id<Movie> movieId,
        Guid kindId,
        FranchiseKindSource kindSource,
        bool showTitles = false,
        bool isLooselyConnected = false,
        bool continueNumbering = false) =>
        this.CreateFranchiseRequest(
            kindId,
            kindSource,
            showTitles,
            isLooselyConnected,
            continueNumbering,
            new FranchiseItemRequest(movieId.Value, FranchiseItemType.Movie, 1, true));

    private FranchiseRequest CreateFranchiseRequest(
        Guid kindId,
        FranchiseKindSource kindSource,
        bool showTitles = false,
        bool isLooselyConnected = false,
        bool continueNumbering = false,
        params FranchiseItemRequest[] items) =>
        new(
            data.ListId.Value,
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(new TitleRequest("Test", 1)).AsValue(),
            ImmutableList.Create(items).AsValue(),
            kindId,
            kindSource,
            showTitles,
            isLooselyConnected,
            continueNumbering);
}
