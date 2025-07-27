using Cineaste.Shared.Models.List;

using Microsoft.EntityFrameworkCore;

namespace Cineaste.Services;

public class ListServiceTests(DataFixture data, ITestOutputHelper output)
{
    private readonly ILogger<ListService> logger = XUnitLogger.Create<ListService>(output);

    [Fact(DisplayName = "GetList should return a correct list")]
    public async Task GetListShouldReturnCorrectList()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        var list = await data.GetList(dbContext);

        // Act

        var model = await listService.GetList(TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(list.Id.Value, model.Id);
        Assert.Equal(list.Configuration.Culture.ToString(), model.Config.Culture);
        Assert.Equal(list.Configuration.DefaultSeasonTitle, model.Config.DefaultSeasonTitle);
        Assert.Equal(list.Configuration.DefaultSeasonOriginalTitle, model.Config.DefaultSeasonOriginalTitle);
    }

    [Fact(DisplayName = "GetListItems should return correct items")]
    public async Task GetListItemsShouldReturnCorrectItems()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        dbContext.ListItems.RemoveRange(dbContext.ListItems);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var list = await data.GetList(dbContext);

        await data.CreateMovie(dbContext);
        await data.CreateMovie(dbContext);
        await data.CreateSeries(dbContext);
        await data.CreateSeries(dbContext);

        int offset = 0;
        int size = list.Items.Count + 10;

        // Act

        var model = await listService.GetListItems(offset, size, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(list.Items.Count, model.Items.Count);
        Assert.Equal(offset, model.Metadata.Offset);
        Assert.Equal(size, model.Metadata.Size);
        Assert.Equal(list.Items.Count, model.Metadata.TotalItems);

        foreach (var (expectedItem, actualItem) in list.Items.Zip(model.Items))
        {
            Assert.Equal(expectedItem.GetId(), actualItem.Id);
            Assert.Equal(expectedItem.GetTitle(), actualItem.Title);
            Assert.Equal(expectedItem.GetOriginalTitle(), actualItem.OriginalTitle);
            Assert.Equal(expectedItem.StartYear, actualItem.StartYear);
            Assert.Equal(expectedItem.EndYear, actualItem.EndYear);
            Assert.Equal(expectedItem.GetColor()?.HexValue ?? String.Empty, actualItem.Color);
            Assert.Equal(expectedItem.SequenceNumber, actualItem.ListSequenceNumber);
            Assert.Equal(expectedItem.GetParentFranchise()?.Id.Value, actualItem.FranchiseItem?.FranchiseId);
            Assert.Equal(expectedItem.GetFranchiseSequenceNumber(), actualItem.FranchiseItem?.SequenceNumber);
            Assert.Equal(expectedItem.GetFranchiseDisplayNumber(), actualItem.FranchiseItem?.DisplayNumber);
            Assert.Equal(
                expectedItem.GetParentFranchise()?.IsLooselyConnected, actualItem.FranchiseItem?.IsLooselyConnected);
        }
    }

    [Fact(DisplayName = "GetListItem should return correct item")]
    public async Task GetListItemShouldReturnCorrectItem()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        dbContext.ListItems.RemoveRange(dbContext.ListItems);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var movie = await data.CreateMovie(dbContext);
        await data.CreateMovie(dbContext);
        await data.CreateSeries(dbContext);
        await data.CreateSeries(dbContext);

        await dbContext.ListItems.ToListAsync(TestContext.Current.CancellationToken);
        await dbContext.FranchiseItems.ToListAsync(TestContext.Current.CancellationToken);
        await dbContext.Franchises.ToListAsync(TestContext.Current.CancellationToken);

        // Act

        var model = await listService.GetListItem(movie.Id.Value, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(movie.Id.Value, model.Id);
        Assert.Equal(ListItemType.Movie, model.Type);
        Assert.Equal(movie.Title.Name, model.Title);
        Assert.Equal(movie.OriginalTitle.Name, model.OriginalTitle);
        Assert.Equal(movie.Year, model.StartYear);
        Assert.Equal(movie.Year, model.EndYear);
        Assert.Equal(movie.GetActiveColor().HexValue ?? String.Empty, model.Color);
        Assert.Equal(movie.ListItem!.SequenceNumber, model.ListSequenceNumber);

        if (movie.FranchiseItem is { } franchiseItem)
        {
            Assert.NotNull(model.FranchiseItem);
            Assert.Equal(franchiseItem.ParentFranchise.Id.Value, model.FranchiseItem.FranchiseId);
            Assert.Equal(franchiseItem.SequenceNumber, model.FranchiseItem.SequenceNumber);
            Assert.Equal(franchiseItem.DisplayNumber, model.FranchiseItem.DisplayNumber);
            Assert.Equal(franchiseItem.ParentFranchise.IsLooselyConnected, model.FranchiseItem.IsLooselyConnected);
        } else
        {
            Assert.Null(model.FranchiseItem);
        }
    }

    [Fact(DisplayName = "GetListItemByParentFranchise should return correct item")]
    public async Task GetListItemByParentFranchiseShouldReturnCorrectItem()
    {
        // Arrange

        var dbContext = data.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        dbContext.ListItems.RemoveRange(dbContext.ListItems);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var movie = await data.CreateMovie(dbContext);
        await data.CreateMovie(dbContext);
        await data.CreateSeries(dbContext);
        await data.CreateSeries(dbContext);

        var franchise = await data.CreateFranchise(dbContext);
        franchise.AttachMovie(movie, true);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await dbContext.ListItems.ToListAsync(TestContext.Current.CancellationToken);
        await dbContext.FranchiseItems.ToListAsync(TestContext.Current.CancellationToken);
        await dbContext.Franchises.ToListAsync(TestContext.Current.CancellationToken);

        // Act

        var model = await listService.GetListItemByParentFranchise(
            franchise.Id.Value, 1, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(movie.Id.Value, model.Id);
        Assert.Equal(ListItemType.Movie, model.Type);
        Assert.Equal(movie.Title.Name, model.Title);
        Assert.Equal(movie.OriginalTitle.Name, model.OriginalTitle);
        Assert.Equal(movie.Year, model.StartYear);
        Assert.Equal(movie.Year, model.EndYear);
        Assert.Equal(movie.GetActiveColor().HexValue ?? String.Empty, model.Color);
        Assert.Equal(movie.ListItem!.SequenceNumber, model.ListSequenceNumber);

        if (movie.FranchiseItem is { } franchiseItem)
        {
            Assert.NotNull(model.FranchiseItem);
            Assert.Equal(franchiseItem.ParentFranchise.Id.Value, model.FranchiseItem.FranchiseId);
            Assert.Equal(franchiseItem.SequenceNumber, model.FranchiseItem.SequenceNumber);
            Assert.Equal(franchiseItem.DisplayNumber, model.FranchiseItem.DisplayNumber);
            Assert.Equal(franchiseItem.ParentFranchise.IsLooselyConnected, model.FranchiseItem.IsLooselyConnected);
        } else
        {
            Assert.Null(model.FranchiseItem);
        }
    }
}

file static class Extensions
{
    public static Guid GetId(this ListItem item) =>
        item.Select(
            movie => movie.Id.Value,
            series => series.Id.Value,
            franchise => franchise.Id.Value);

    public static string GetTitle(this ListItem item) =>
        item.Select(
            movie => movie.Title.Name,
            series => series.Title.Name,
            franchise => franchise.ShowTitles && franchise.Title is not null
                ? $"{franchise.Title.Name}:"
                : String.Empty);

    public static string GetOriginalTitle(this ListItem item) =>
        item.Select(
            movie => movie.OriginalTitle.Name,
            series => series.OriginalTitle.Name,
            franchise => franchise.ShowTitles && franchise.OriginalTitle is not null
                ? $"{franchise.OriginalTitle.Name}:"
                : String.Empty);

    public static Franchise? GetParentFranchise(this ListItem item) =>
        item.Select(
            movie => movie.FranchiseItem?.ParentFranchise,
            series => series.FranchiseItem?.ParentFranchise,
            franchise => franchise.FranchiseItem?.ParentFranchise);

    public static int? GetFranchiseSequenceNumber(this ListItem item) =>
        item.Select(
            movie => movie.FranchiseItem?.SequenceNumber,
            series => series.FranchiseItem?.SequenceNumber,
            franchise => franchise.FranchiseItem?.SequenceNumber);

    public static int? GetFranchiseDisplayNumber(this ListItem item) =>
        item.Select(
            movie => movie.FranchiseItem?.DisplayNumber,
            series => series.FranchiseItem?.DisplayNumber,
            franchise => franchise.FranchiseItem?.DisplayNumber);

    public static Color? GetColor(this ListItem item) =>
        item.Select(
            movie => movie.GetActiveColor(),
            series => series.GetActiveColor(),
            franchise => franchise.GetActiveColor());
}
