namespace Cineaste.Server.Services;

using System.Globalization;

using Cineaste.Basic;
using Cineaste.Core;
using Cineaste.Core.Domain;
using Cineaste.Persistence;
using Cineaste.Server.Exceptions;
using Cineaste.Shared.Models.Franchise;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class FranchiseServiceTests : ServiceTestsBase
{
    private readonly CineasteList list;
    private readonly Movie movie;
    private readonly Series series;
    private readonly MovieKind movieKind;
    private readonly SeriesKind seriesKind;
    private readonly ILogger<FranchiseService> logger;

    public FranchiseServiceTests()
    {
        this.list = this.CreateList();

        this.movieKind = this.CreateMovieKind(this.list);
        this.seriesKind = this.CreateSeriesKind(this.list);

        this.movie = this.CreateMovie(this.list);
        this.series = this.CreateSeries(this.list);

        this.logger = new NullLogger<FranchiseService>();
    }

    [Fact(
        DisplayName = "GetFranchise should return the correct franchise",
        Skip = "EF in-memory provider doesn't work")]
    public async Task GetFranchiseShouldReturnCorrectFranchise()
    {
        var dbContext = this.CreateInMemoryDb();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var franchise = this.CreateFranchise();
        this.list.AddFranchise(franchise);

        dbContext.Franchises.Add(franchise);
        dbContext.SaveChanges();

        var model = await franchiseService.GetFranchise(franchise.Id);

        this.AssertTitles(franchise, model);

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

    [Fact(
        DisplayName = "GetFranchise should throw if franchise isn't found",
        Skip = "EF in-memory provider doesn't work")]
    public async Task GetFranchiseShouldThrowIfNotFound()
    {
        var dbContext = this.CreateInMemoryDb();
        var franchiseService = new FranchiseService(dbContext, this.logger);

        this.SetUpDb(dbContext);

        var dummyId = Id.CreateNew<Franchise>();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => franchiseService.GetFranchise(dummyId));

        Assert.Equal("NotFound.Franchise", exception.MessageCode);
        Assert.Equal("Resource.Franchise", exception.Resource);
        Assert.Equal(dummyId, exception.Properties["id"]);
    }

    private Franchise CreateFranchise()
    {
        var franchise = new Franchise(
            Id.CreateNew<Franchise>(),
            this.CreateTitles(),
            showTitles: true,
            isLooselyConnected: false,
            continueNumbering: false);

        franchise.AddMovie(this.movie);
        franchise.AddSeries(this.series);

        return franchise;
    }

    private IEnumerable<Title> CreateTitles() =>
        new List<Title> { new("Test", 1, false), new("Original Test", 1, true) };

    private Movie CreateMovie(CineasteList list)
    {
        var movie = new Movie(Id.CreateNew<Movie>(), this.CreateTitles(), 2000, true, true, this.movieKind);
        list.AddMovie(movie);

        return movie;
    }

    private Series CreateSeries(CineasteList list)
    {
        var series = new Series(
            Id.CreateNew<Series>(),
            this.CreateTitles(),
            new List<Season> { this.CreateSeason(1), this.CreateSeason(2) },
            new List<SpecialEpisode> { this.CreateSpecialEpisode(3) },
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            this.seriesKind);

        list.AddSeries(series);

        return series;
    }

    private Season CreateSeason(int num) =>
        new(
            Id.CreateNew<Season>(),
            new List<Title> { new($"Season {num}", 1, false), new($"Season {num}", 1, true) },
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.Finished,
            "Test",
            num,
            new List<Period>
            {
                new(Id.CreateNew<Period>(), 1, 2000 + num, 2, 2000 + num, false, 10)
            });

    private SpecialEpisode CreateSpecialEpisode(int num) =>
        new(Id.CreateNew<SpecialEpisode>(), this.CreateTitles(), 1, 2000 + num, false, true, "Test", num);

    private MovieKind CreateMovieKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new MovieKind(Id.CreateNew<MovieKind>(), "Test Kind", black, black, black);

        list.AddMovieKind(kind);

        return kind;
    }

    private SeriesKind CreateSeriesKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new SeriesKind(Id.CreateNew<SeriesKind>(), "Test Kind", black, black, black);

        list.AddSeriesKind(kind);

        return kind;
    }

    private CineasteList CreateList() =>
        new(
            Id.CreateNew<CineasteList>(),
            "Test List",
            "test-list",
            new ListConfiguration(
                Id.CreateNew<ListConfiguration>(),
                CultureInfo.InvariantCulture,
                "Season #",
                "Season #",
                ListSortingConfiguration.CreateDefault()));

    private void SetUpDb(CineasteDbContext context)
    {
        context.MovieKinds.Add(this.movieKind);
        context.SeriesKinds.Add(this.seriesKind);
        context.Movies.Add(this.movie);
        context.Series.Add(this.series);
        context.Lists.Add(this.list);

        context.SaveChanges();
    }
}
