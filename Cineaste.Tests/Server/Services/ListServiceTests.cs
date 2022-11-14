namespace Cineaste.Server.Services;

using System.Globalization;

using Cineaste.Basic;
using Cineaste.Core.Domain;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.List;
using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Validation;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public class ListServiceTests : ServiceTestsBase
{
    private readonly ILogger<ListService> logger;

    public ListServiceTests() =>
        this.logger = new NullLogger<ListService>();

    [Fact(DisplayName = "GetAllLists should return correct lists")]
    public async Task GetAllListsShouldReturnCorrectLists()
    {
        var dbContext = this.CreateInMemoryDb();
        var listService = new ListService(dbContext, this.logger);

        var lists = new List<CineasteList>
        {
            this.CreateList("Test List 1", "test-list-1"),
            this.CreateList("Test List 2", "test-list-2"),
            this.CreateList("Test List 3", "test-list-3")
        };

        dbContext.Lists.AddRange(lists);
        dbContext.SaveChanges();

        var listModels = await listService.GetAllLists();

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
        var dbContext = this.CreateInMemoryDb();
        var listService = new ListService(dbContext, this.logger);

        var list = this.CreateList("Test List", "test-list");
        this.PopulateList(list);

        dbContext.Lists.Add(list);
        dbContext.SaveChanges();

        var model = await listService.GetList(list.Handle);

        Assert.Equal(list.Id.Value, model.Id);
        Assert.Equal(list.Name, model.Name);
        Assert.Equal(list.Handle, model.Handle);
        Assert.Equal(list.Configuration.Culture.ToString(), model.Config.Culture);
        Assert.Equal(list.Configuration.DefaultSeasonTitle, model.Config.DefaultSeasonTitle);
        Assert.Equal(list.Configuration.DefaultSeasonOriginalTitle, model.Config.DefaultSeasonOriginalTitle);

        Assert.Single(model.MovieKinds);
        Assert.Single(model.SeriesKinds);
        Assert.Single(model.Movies);
        Assert.Single(model.Series);
    }

    [Fact(DisplayName = "CreateList should put it into the database")]
    public async Task CreateListShouldPutItIntoDb()
    {
        var dbContext = this.CreateInMemoryDb();
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
        var dbContext = this.CreateInMemoryDb();
        var listService = new ListService(dbContext, this.logger);

        var request = new CreateListRequest("Test List", String.Empty, "Season #", "Season #");

        var model = await listService.CreateList(request.Validated());
        
        Assert.Equal(request.Name, model.Name);
        Assert.Equal("test-list", model.Handle);
    }

    [Fact(DisplayName = "GetList should throw if not found")]
    public async Task GetListShouldThrowIfNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var listService = new ListService(dbContext, this.logger);

        string dummyHandle = "test";

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => listService.GetList(dummyHandle));

        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(dummyHandle, exception.Properties["handle"]);
    }

    [Fact(DisplayName = "CreateList should throw if handle already exists")]
    public async Task CreateListShouldThrowIfHandleExists()
    {
        var dbContext = this.CreateInMemoryDb();
        var listService = new ListService(dbContext, this.logger);

        const string name = "Test List";
        const string handle = "test-list";

        dbContext.Lists.Add(this.CreateList(name, handle));
        dbContext.SaveChanges();

        var request = new CreateListRequest(name, String.Empty, "Season #", "Season #");

        var exception = await Assert.ThrowsAsync<ConflictException>(() => listService.CreateList(request.Validated()));

        Assert.Equal("Resource.List", exception.Resource);
        Assert.Equal(handle, exception.Properties["handle"]);
    }

    private CineasteList CreateList(string name, string handle) =>
        new(
            Id.CreateNew<CineasteList>(),
            name,
            handle,
            new ListConfiguration(
                Id.CreateNew<ListConfiguration>(),
                CultureInfo.InvariantCulture,
                "Season #",
                "Season #",
                ListSortingConfiguration.CreateDefault()));

    private void PopulateList(CineasteList list)
    {
        var black = new Color("#000000");

        var movieKind = new MovieKind(Id.CreateNew<MovieKind>(), "Test Kind", black, black, black);
        list.AddMovieKind(movieKind);

        var seriesKind = new SeriesKind(Id.CreateNew<SeriesKind>(), "Test Kind", black, black, black);
        list.AddSeriesKind(seriesKind);

        var movie = new Movie(Id.CreateNew<Movie>(), this.CreateTitles(), 2000, false, true, movieKind);
        list.AddMovie(movie);

        var series = new Series(
            Id.CreateNew<Series>(),
            this.CreateTitles(),
            new List<Season>
            {
                new(
                    Id.CreateNew<Season>(),
                    this.CreateTitles(),
                    SeasonWatchStatus.NotWatched,
                    SeasonReleaseStatus.Finished,
                    "Test",
                    1,
                    new List<Period> { new(Id.CreateNew<Period>(), 1, 2000, 2, 2000, false, 10) })
            },
            Enumerable.Empty<SpecialEpisode>(),
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            seriesKind);

        list.AddSeries(series);
    }

    private IEnumerable<Title> CreateTitles() =>
        new List<Title> { new("Test", 1, false), new("Original Test", 1, true) };
}
