namespace Cineaste.Server.Services;

using System.Globalization;

using Cineaste.Core.Domain;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.List;
using Cineaste.Shared.Validation;

using Microsoft.Extensions.Logging;

public class ListServiceTests : ServiceTestsBase
{
    private readonly ILogger<ListService> logger;

    public ListServiceTests(ITestOutputHelper output)
        : base(output) =>
        this.logger = XUnitLogger.CreateLogger<ListService>(output);

    [Fact(DisplayName = "GetAllLists should return correct lists")]
    public async Task GetAllListsShouldReturnCorrectLists()
    {
        var dbContext = await this.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        var lists = new List<CineasteList>
        {
            this.CreateList("Test List 1", "test-list-1"),
            this.CreateList("Test List 2", "test-list-2"),
            this.CreateList("Test List 3", "test-list-3")
        };

        dbContext.Lists.AddRange(lists);
        dbContext.Lists.Remove(this.List);
        await dbContext.SaveChangesAsync();

        var listModels = await listService.GetAllLists();

        dbContext.Lists.Add(this.List);
        await dbContext.SaveChangesAsync();

        Assert.Equal(lists.Count, listModels.Count);

        foreach (var (list, listModel) in lists.Zip(listModels))
        {
            Assert.Equal(list.Id.Value, listModel.Id);
            Assert.Equal(list.Name, listModel.Name);
            Assert.Equal(list.Handle, listModel.Handle);
        }
    }

    [Fact(DisplayName = "GetList should return a correct list")]
    public async Task GetListShouldReturnCorrectList()
    {
        var dbContext = await this.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        var model = await listService.GetList(this.List.Handle);

        Assert.Equal(this.List.Id.Value, model.Id);
        Assert.Equal(this.List.Name, model.Name);
        Assert.Equal(this.List.Handle, model.Handle);
        Assert.Equal(this.List.Configuration.Culture.ToString(), model.Config.Culture);
        Assert.Equal(this.List.Configuration.DefaultSeasonTitle, model.Config.DefaultSeasonTitle);
        Assert.Equal(this.List.Configuration.DefaultSeasonOriginalTitle, model.Config.DefaultSeasonOriginalTitle);
    }

    [Fact(DisplayName = "CreateList should put it into the database")]
    public async Task CreateListShouldPutItIntoDb()
    {
        var dbContext = await this.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        var request = new CreateListRequest("Test List", String.Empty, "Season #", "Season #");

        var model = await listService.CreateList(request.Validated());

        var list = dbContext.Lists.Find(Id.Create<CineasteList>(model.Id));

        Assert.NotNull(list);
        Assert.NotNull(list.Configuration);

        Assert.Equal(request.Name, list.Name);
        Assert.Equal("test-list", list.Handle);
        Assert.Equal(CultureInfo.InvariantCulture, list.Configuration.Culture);
        Assert.Equal(request.DefaultSeasonTitle, list.Configuration.DefaultSeasonTitle);
        Assert.Equal(request.DefaultSeasonOriginalTitle, list.Configuration.DefaultSeasonOriginalTitle);
    }

    [Fact(DisplayName = "CreateList should return a correct model")]
    public async Task CreateListShouldReturnCorrectModel()
    {
        var dbContext = await this.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        var request = new CreateListRequest("Test List", String.Empty, "Season #", "Season #");

        var model = await listService.CreateList(request.Validated());
        
        Assert.Equal(request.Name, model.Name);
        Assert.Equal("test-list", model.Handle);
    }

    [Fact(DisplayName = "GetList should throw if not found")]
    public async Task GetListShouldThrowIfNotFound()
    {
        var dbContext = await this.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        string dummyHandle = "test";

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => listService.GetList(dummyHandle));

        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyHandle, exception.Properties["handle"]);
    }

    [Fact(DisplayName = "CreateList should throw if handle already exists")]
    public async Task CreateListShouldThrowIfHandleExists()
    {
        var dbContext = await this.CreateDbContext();
        var listService = new ListService(dbContext, this.logger);

        const string name = "Test List";
        const string handle = "test-list";

        dbContext.Lists.Add(this.CreateList(name, handle));
        await dbContext.SaveChangesAsync();

        var request = new CreateListRequest(name, String.Empty, "Season #", "Season #");

        var exception = await Assert.ThrowsAsync<ConflictException>(() => listService.CreateList(request.Validated()));

        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(handle, exception.Properties["handle"]);
    }
}
