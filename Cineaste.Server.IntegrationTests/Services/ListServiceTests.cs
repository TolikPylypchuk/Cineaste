namespace Cineaste.Server.Services;

using Microsoft.Extensions.Logging;

public class ListServiceTests : ServiceTestsBase
{
    private readonly ILogger<ListService> logger;

    public ListServiceTests(ITestOutputHelper output) =>
        this.logger = XUnitLogger.Create<ListService>(output);

    [Fact(DisplayName = "GetList should return a correct list")]
    public async Task GetListShouldReturnCorrectList()
    {
        // Arrange

        var dbContext = await this.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        // Act

        var model = await listService.GetList();

        // Assert

        Assert.Equal(this.List.Id.Value, model.Id);
        Assert.Equal(this.List.Configuration.Culture.ToString(), model.Config.Culture);
        Assert.Equal(this.List.Configuration.DefaultSeasonTitle, model.Config.DefaultSeasonTitle);
        Assert.Equal(this.List.Configuration.DefaultSeasonOriginalTitle, model.Config.DefaultSeasonOriginalTitle);
    }
}
