namespace Cineaste.Server.Services;

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
}
