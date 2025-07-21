using System.Globalization;

namespace Cineaste.Data;

internal sealed class TestDataProvider(CineasteDbContext dbContext)
{
    public async Task CreateTestDataIfNeeded()
    {
        bool listsPresentInDb = await dbContext.Lists.AnyAsync();

        if (!listsPresentInDb)
        {
            var list = this.CreateList();
            dbContext.Lists.Add(list);
            await dbContext.SaveChangesAsync();
        }
    }

    private CineasteList CreateList()
    {
        var config = new ListConfiguration(
            Id.Create<ListConfiguration>(),
            CultureInfo.InvariantCulture,
            "Season #",
            "Season #",
            ListSortingConfiguration.CreateDefault());

        var list = new CineasteList(Id.Create<CineasteList>(), config);

        var black = new Color("#000000");
        var blue = new Color("#3949ab");
        var green = new Color("#43a047");
        var lightBlue = new Color("#1e88e5");
        var red = new Color("#e35953");
        var darkRed = new Color("#b71c1c");

        var liveActionMovie = new MovieKind(Id.Create<MovieKind>(), "Live-Action", black, red, darkRed);
        var animatedMovie = new MovieKind(Id.Create<MovieKind>(), "Animation", green, red, darkRed);
        var liveActionSeries = new SeriesKind(Id.Create<SeriesKind>(), "Live-Action", blue, red, darkRed);
        var animatedSeries = new SeriesKind(Id.Create<SeriesKind>(), "Animation", lightBlue, red, darkRed);

        list.AddMovieKind(liveActionMovie);
        list.AddMovieKind(animatedMovie);
        list.AddSeriesKind(liveActionSeries);
        list.AddSeriesKind(animatedSeries);

        list.AddMovie(this.CreateMovie("12 Angry Men", liveActionMovie, 1957));
        list.AddMovie(this.CreateMovie("Anatomy of a Fall", "Anatomie d'une chute", liveActionMovie, 2023));
        list.AddMovie(this.CreateMovie("Poor Things", liveActionMovie, 2023));
        list.AddMovie(this.CreateMovie("The Departed", liveActionMovie, 2006));
        list.AddMovie(this.CreateMovie("Up", animatedMovie, 2009));
        list.AddMovie(this.CreateMovie("Everything Everywhere All at Once", liveActionMovie, 2022));
        list.AddMovie(this.CreateMovie("The Grand Budapest Hotel", liveActionMovie, 2014));
        list.AddMovie(this.CreateMovie("Soul", animatedMovie, 2020));
        list.AddMovie(this.CreateMovie("Tangled", animatedMovie, 2010));
        list.AddMovie(this.CreateMovie("Pulp Fiction", animatedMovie, 1994));
        list.AddMovie(this.CreateMovie("The Intouchables", "Intouchables", liveActionMovie, 2011));
        list.AddMovie(this.CreateMovie("Amélie", "Le fabuleux destin d'Amélie Poulain", liveActionMovie, 2001));
        list.AddMovie(this.CreateMovie("Ratatouille", animatedMovie, 2007));
        list.AddMovie(this.CreateMovie("Scott Pilgrim vs. the World", liveActionMovie, 2010));

        list.AddSeries(this.BandOfBrothers(liveActionSeries));
        list.AddSeries(this.MrRobot(liveActionSeries));
        list.AddSeries(this.Sherlock(liveActionSeries));

        var lotr = this.CreateFranchise("The Lord of the Rings", showTitles: false);

        var lotr1 = this.CreateMovie("The Lord of the Rings: The Fellowship of the Ring", liveActionMovie, 2001);
        var lotr2 = this.CreateMovie("The Lord of the Rings: The Two Towers", liveActionMovie, 2002);
        var lotr3 = this.CreateMovie("The Lord of the Rings: The Return of the King", liveActionMovie, 2003);

        lotr.AddMovie(lotr1, true);
        lotr.AddMovie(lotr2, true);
        lotr.AddMovie(lotr3, true);

        list.AddFranchise(lotr);
        list.AddMovie(lotr1);
        list.AddMovie(lotr2);
        list.AddMovie(lotr3);

        var dollars = this.CreateFranchise("Dollars Trilogy", isLooselyConnected: true);

        var dollars1 = this.CreateMovie("A Fistful of Dollars", "Per un pugno di dollari", liveActionMovie, 1964);
        var dollars2 = this.CreateMovie("For a Few Dollars More", "Per qualche dollaro in più", liveActionMovie, 1965);
        var dollars3 = this.CreateMovie(
            "The Good, the Bad and the Ugly", "Il buono, il brutto, il cattivo", liveActionMovie, 1966);

        dollars.AddMovie(dollars1, true);
        dollars.AddMovie(dollars2, true);
        dollars.AddMovie(dollars3, true);

        list.AddFranchise(dollars);
        list.AddMovie(dollars1);
        list.AddMovie(dollars2);
        list.AddMovie(dollars3);

        var dune = this.CreateFranchise("Dune", showTitles: false);

        var dune1 = this.CreateMovie("Dune", liveActionMovie, 2021);
        var dune2 = this.CreateMovie("Dune: Part Two", liveActionMovie, 1965);
        var duneProphecy = this.DuneProphecy(liveActionSeries);
        var dune3 = this.CreateMovie("Dune: Part Three", liveActionMovie, 2026, isReleased: false);

        dune.AddMovie(dune1, true);
        dune.AddMovie(dune2, true);
        dune.AddSeries(duneProphecy, false);
        dune.AddMovie(dune3, true);

        list.AddFranchise(dune);
        list.AddMovie(dune1);
        list.AddMovie(dune2);
        list.AddSeries(duneProphecy);
        list.AddMovie(dune3);

        var dc = this.CreateFranchise("DC Universe");
        var godsAndMonsters = this.CreateFranchise("Chapter One: Gods and Monsters");
        dc.AddFranchise(godsAndMonsters, true);

        var creatureCommandos = this.CreatureCommandos(animatedSeries);
        var superman = this.CreateMovie("Superman", liveActionMovie, 2025);
        var supergirl = this.CreateMovie("Supergirl", liveActionMovie, 2026, isReleased: false);

        godsAndMonsters.AddSeries(creatureCommandos, true);
        godsAndMonsters.AddMovie(superman, true);
        godsAndMonsters.AddMovie(supergirl, true);

        list.AddFranchise(dc);
        list.AddFranchise(godsAndMonsters);
        list.AddSeries(creatureCommandos);
        list.AddMovie(superman);
        list.AddMovie(supergirl);

        list.SortItems();

        return list;
    }

    private Movie CreateMovie(string title, MovieKind kind, int year, bool isReleased = true) =>
        this.CreateMovie(title, title, kind, year, isReleased);

    private Movie CreateMovie(string title, string originalTitle, MovieKind kind, int year, bool isReleased = true) =>
        new(
            Id.Create<Movie>(),
            [new(title, 1, false), new(originalTitle, 1, true)],
            year,
            isWatched: isReleased,
            isReleased: isReleased,
            kind);

    private Series BandOfBrothers(SeriesKind kind)
    {
        const string title = "Band of Brothers";

        var period = new Period(Id.Create<Period>(), 9, 2001, 11, 2001, false, 10);

        var season = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            "HBO",
            sequenceNumber: 1,
            [period]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season],
            [],
            SeriesWatchStatus.Watched,
            SeriesReleaseStatus.Finished,
            kind);
    }

    private Series DuneProphecy(SeriesKind kind)
    {
        const string title = "Dune: Prophecy";
        const string channel = "HBO";

        var period1 = new Period(Id.Create<Period>(), 11, 2024, 12, 2024, false, 6);
        var period2 = new Period(Id.Create<Period>(), 11, 2026, 12, 2026, false, 6);

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [period1]);

        var season2 = new Season(
            Id.Create<Season>(),
            [new("Season 2", 1, false), new("Season 2", 1, true)],
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.NotStarted,
            channel,
            sequenceNumber: 2,
            [period2]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1, season2],
            [],
            SeriesWatchStatus.Watching,
            SeriesReleaseStatus.Running,
            kind);
    }

    private Series MrRobot(SeriesKind kind)
    {
        const string title = "Mr. Robot";
        const string channel = "USA";

        var period1 = new Period(Id.Create<Period>(), 6, 2015, 9, 2015, false, 10);
        var period2 = new Period(Id.Create<Period>(), 7, 2016, 9, 2016, false, 12);
        var period3 = new Period(Id.Create<Period>(), 10, 2017, 12, 2017, false, 10);
        var period4 = new Period(Id.Create<Period>(), 10, 2019, 12, 2019, false, 13);

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [period1]);

        var season2 = new Season(
            Id.Create<Season>(),
            [new("Season 2", 1, false), new("Season 2", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 2,
            [period2]);

        var season3 = new Season(
            Id.Create<Season>(),
            [new("Season 3", 1, false), new("Season 3", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 3,
            [period3]);

        var season4 = new Season(
            Id.Create<Season>(),
            [new("Season 4", 1, false), new("Season 4", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 4,
            [period4]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1, season2, season3, season4],
            [],
            SeriesWatchStatus.Watched,
            SeriesReleaseStatus.Finished,
            kind);
    }

    private Series Sherlock(SeriesKind kind)
    {
        const string title = "Sherlock";
        const string channel = "BBC One";

        var period1 = new Period(Id.Create<Period>(), 7, 2010, 8, 2010, false, 3);
        var period2 = new Period(Id.Create<Period>(), 1, 2012, 1, 2012, false, 3);
        var period3 = new Period(Id.Create<Period>(), 1, 2014, 1, 2014, false, 3);
        var period4 = new Period(Id.Create<Period>(), 1, 2017, 1, 2017, false, 3);

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [period1]);

        var season2 = new Season(
            Id.Create<Season>(),
            [new("Season 2", 1, false), new("Season 2", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 2,
            [period2]);

        var season3 = new Season(
            Id.Create<Season>(),
            [new("Season 3", 1, false), new("Season 3", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 3,
            [period3]);

        var episode = new SpecialEpisode(
            Id.Create<SpecialEpisode>(),
            [new("The Abominable Bride", 1, false), new("The Abominable Bride", 1, true)],
            month: 1,
            year: 2016,
            isWatched: true,
            isReleased: true,
            channel,
            sequenceNumber: 4);

        var season4 = new Season(
            Id.Create<Season>(),
            [new("Season 4", 1, false), new("Season 4", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 5,
            [period4]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1, season2, season3, season4],
            [episode],
            SeriesWatchStatus.Watched,
            SeriesReleaseStatus.Finished,
            kind);
    }

    private Series CreatureCommandos(SeriesKind kind)
    {
        const string title = "Creature Commandos";
        const string channel = "BBC One";

        var period1 = new Period(Id.Create<Period>(), 12, 2024, 1, 2025, false, 7);
        var period2 = new Period(Id.Create<Period>(), 1, 2026, 2, 2026, false, 7);

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [period1]);

        var season2 = new Season(
            Id.Create<Season>(),
            [new("Season 2", 1, false), new("Season 2", 1, true)],
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.NotStarted,
            channel,
            sequenceNumber: 2,
            [period2]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1, season2],
            [],
            SeriesWatchStatus.Watching,
            SeriesReleaseStatus.Finished,
            kind);
    }

    private Franchise CreateFranchise(string? title = null, bool? showTitles = null, bool isLooselyConnected = false) =>
        new(
            Id.Create<Franchise>(),
            title is not null ? [new(title, 1, false), new(title, 1, true)] : new List<Title>(),
            showTitles: showTitles ?? title is not null,
            isLooselyConnected: isLooselyConnected,
            continueNumbering: false);
}
