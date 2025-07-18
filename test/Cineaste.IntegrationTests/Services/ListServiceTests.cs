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

        var model = await listService.GetList();

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

        var model = await listService.GetListItems(offset, size);

        // Assert

        Assert.Equal(list.Items.Count, model.Items.Count);
        Assert.Equal(offset, model.Metadata.Offset);
        Assert.Equal(size, model.Metadata.Size);
        Assert.Equal(list.Items.Count, model.Metadata.TotalItems);

        foreach (var (expectedItem, actualItem) in list.Items.Zip(model.Items))
        {
            Assert.Equal(this.GetId(expectedItem), actualItem.Id);
            Assert.Equal(this.GetTitle(expectedItem), actualItem.Title);
            Assert.Equal(this.GetOriginalTitle(expectedItem), actualItem.OriginalTitle);
            Assert.Equal(this.GetDisplayNumber(expectedItem), actualItem.FranchiseItem?.DisplayNumber);
        }
    }

    private Guid GetId(ListItem item) =>
        item.Select(
            movie => movie.Id.Value,
            series => series.Id.Value,
            franchise => franchise.Id.Value);

    private string GetTitle(ListItem item) =>
        item.Select(
            movie => movie.Title.Name,
            series => series.Title.Name,
            franchise => franchise.ShowTitles && franchise.Title is not null
                ? $"{franchise.Title.Name}:"
                : String.Empty);

    private string GetOriginalTitle(ListItem item) =>
        item.Select(
            movie => movie.OriginalTitle.Name,
            series => series.OriginalTitle.Name,
            franchise => franchise.ShowTitles && franchise.OriginalTitle is not null
                ? $"{franchise.OriginalTitle.Name}:"
                : String.Empty);

    private int? GetDisplayNumber(ListItem item) =>
        item.Select(
            movie => movie.FranchiseItem?.DisplayNumber,
            series => series.FranchiseItem?.DisplayNumber,
            franchise => franchise.FranchiseItem?.DisplayNumber);
}
