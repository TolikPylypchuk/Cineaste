using Cineaste.Application.Services.User;

using Microsoft.AspNetCore.Identity;

namespace Cineaste.Data;

internal sealed partial class TestDataProvider(
    CineasteDbContext dbContext,
    IUserRegistrationService userRegistrationService,
    ILogger<TestDataProvider> logger)
{
    private readonly CineasteDbContext dbContext = dbContext;
    private readonly IUserRegistrationService userRegistrationService = userRegistrationService;
    private readonly ILogger<TestDataProvider> logger = logger;

    public async Task CreateTestDataIfNeeded()
    {
        bool listsPresentInDb = await this.dbContext.Lists.AnyAsync();

        if (listsPresentInDb)
        {
            return;
        }

        var strategy = this.dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await this.dbContext.Database.BeginTransactionAsync();

            try
            {
                var invitationCode = new CineasteUserInvitationCode(Id.Create<CineasteUserInvitationCode>());
                this.dbContext.UserInvitationCodes.Add(invitationCode);

                await this.dbContext.SaveChangesAsync();

                var (user, result) = await this.userRegistrationService.RegisterUser(
                    "test@email.com", "123Password!", invitationCode.Id.Value);

                if (!result.Succeeded)
                {
                    if (this.logger.IsEnabled(LogLevel.Error))
                    {
                        foreach (var error in result.Errors)
                        {
                            this.LogUserRegistrationError(error);
                        }
                    }

                    return;
                }

                this.FillList(user.List);

                await this.dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            } catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    private void FillList(CineasteList list)
    {
        var liveActionMovie = list.MovieKinds.First(kind => kind.Name == "Live-Action");
        var animatedMovie = list.MovieKinds.First(kind => kind.Name == "Animation");
        var liveActionSeries = list.SeriesKinds.First(kind => kind.Name == "Live-Action");
        var animatedSeries = list.SeriesKinds.First(kind => kind.Name == "Animation");

        list.AddMovie(this.CreateMovie("12 Angry Men", liveActionMovie, 1957, "tt0050083", "m/1000013-12_angry_men"));
        list.AddMovie(this.CreateMovie(
            "Anatomy of a Fall", "Anatomie d'une chute", liveActionMovie, 2023, "tt17009710", "m/anatomy_of_a_fall"));
        list.AddMovie(this.CreateMovie("Poor Things", liveActionMovie, 2023, "tt14230458", "m/poor_things"));
        list.AddMovie(this.CreateMovie("The Departed", liveActionMovie, 2006, "tt0407887", "m/departed"));
        list.AddMovie(this.CreateMovie("Up", animatedMovie, 2009, "tt1049413", "m/up"));
        list.AddMovie(this.CreateMovie(
            "Everything Everywhere All at Once",
            liveActionMovie,
            2022,
            "tt6710474",
            "m/everything_everywhere_all_at_once"));
        list.AddMovie(this.CreateMovie(
            "The Grand Budapest Hotel", liveActionMovie, 2014, "tt2278388", "m/the_grand_budapest_hotel"));
        list.AddMovie(this.CreateMovie("Soul", animatedMovie, 2020, "tt2948372", "m/soul_2020"));
        list.AddMovie(this.CreateMovie("Tangled", animatedMovie, 2010, "tt0398286", "m/tangled"));
        list.AddMovie(this.CreateMovie("Pulp Fiction", liveActionMovie, 1994, "tt0110912", "m/pulp_fiction"));
        list.AddMovie(this.CreateMovie(
            "The Intouchables", "Intouchables", liveActionMovie, 2011, "tt1675434", "m/the_intouchables"));
        list.AddMovie(this.CreateMovie(
            "Amélie", "Le fabuleux destin d'Amélie Poulain", liveActionMovie, 2001, "tt0211915", "m/amelie"));
        list.AddMovie(this.CreateMovie("Ratatouille", animatedMovie, 2007, "tt0382932", "m/ratatouille"));
        list.AddMovie(this.CreateMovie(
            "Scott Pilgrim vs. the World", liveActionMovie, 2010, "tt0446029", "m/scott_pilgrims_vs_the_world"));

        list.AddLimitedSeries(this.BandOfBrothers(liveActionSeries));
        list.AddSeries(this.MrRobot(liveActionSeries));
        list.AddSeries(this.Sherlock(liveActionSeries));

        var lotr = this.CreateFranchise(
            "The Lord of the Rings", liveActionMovie, liveActionSeries, FranchiseKindSource.Movie, showTitles: false);

        var lotr1 = this.CreateMovie(
            "The Lord of the Rings: The Fellowship of the Ring",
            liveActionMovie,
            2001,
            "tt0120737",
            "m/the_lord_of_the_rings_the_fellowship_of_the_ring");

        var lotr2 = this.CreateMovie(
            "The Lord of the Rings: The Two Towers",
            liveActionMovie,
            2002,
            "tt0167261",
            "m/the_lord_of_the_rings_the_two_towers");

        var lotr3 = this.CreateMovie(
            "The Lord of the Rings: The Return of the King",
            liveActionMovie,
            2003,
            "tt0167260",
            "m/the_lord_of_the_rings_the_return_of_the_king");

        lotr.AttachMovie(lotr1, true);
        lotr.AttachMovie(lotr2, true);
        lotr.AttachMovie(lotr3, true);

        list.AddFranchise(lotr);
        list.AddMovie(lotr1);
        list.AddMovie(lotr2);
        list.AddMovie(lotr3);

        var dollars = this.CreateFranchise(
            "Dollars Trilogy", liveActionMovie, liveActionSeries, FranchiseKindSource.Movie, isLooselyConnected: true);

        var dollars1 = this.CreateMovie(
            "A Fistful of Dollars",
            "Per un pugno di dollari",
            liveActionMovie,
            1964,
            "tt0058461",
            "m/fistful_of_dollars");

        var dollars2 = this.CreateMovie(
            "For a Few Dollars More",
            "Per qualche dollaro in più",
            liveActionMovie,
            1965,
            "tt0059578",
            "m/for_a_few_dollars_more");

        var dollars3 = this.CreateMovie(
            "The Good, the Bad and the Ugly",
            "Il buono, il brutto, il cattivo",
            liveActionMovie,
            1966,
            "tt0060196",
            "m/the_good_the_bad_and_the_ugly");

        dollars.AttachMovie(dollars1, true);
        dollars.AttachMovie(dollars2, true);
        dollars.AttachMovie(dollars3, true);

        list.AddFranchise(dollars);
        list.AddMovie(dollars1);
        list.AddMovie(dollars2);
        list.AddMovie(dollars3);

        var dune = this.CreateFranchise(
            "Dune", liveActionMovie, liveActionSeries, FranchiseKindSource.Movie, showTitles: false);

        var dune1 = this.CreateMovie("Dune", liveActionMovie, 2021, "tt1160419", "m/dune_2020");
        var dune2 = this.CreateMovie("Dune: Part Two", liveActionMovie, 1965, "tt15239678", "m/dune_part_two");
        var duneProphecy = this.DuneProphecy(liveActionSeries);
        var dune3 = this.CreateMovie(
            "Dune: Part Three", liveActionMovie, 2026, "tt3137850", "m/dune_part_three", isReleased: false);

        dune.AttachMovie(dune1, true);
        dune.AttachMovie(dune2, true);
        dune.AttachSeries(duneProphecy, false);
        dune.AttachMovie(dune3, true);

        list.AddFranchise(dune);
        list.AddMovie(dune1);
        list.AddMovie(dune2);
        list.AddSeries(duneProphecy);
        list.AddMovie(dune3);

        var dc = this.CreateFranchise("DC Universe", liveActionMovie, liveActionSeries, FranchiseKindSource.Movie);
        var godsAndMonsters = this.CreateFranchise(
            "Chapter One: Gods and Monsters", liveActionMovie, liveActionSeries, FranchiseKindSource.Movie);

        dc.AttachFranchise(godsAndMonsters, true);

        var creatureCommandos = this.CreatureCommandos(animatedSeries);
        var superman = this.CreateMovie("Superman", liveActionMovie, 2025, "tt5950044", "m/superman_2025");
        var supergirl = this.CreateMovie(
            "Supergirl", liveActionMovie, 2026, "tt8814476", "m/supergirl_2026", isReleased: false);
        var lanterns = this.Lanterns(liveActionSeries);
        var clayface = this.CreateMovie(
            "Clayface", liveActionMovie, 2026, "tt34890576", "m/clayface", isReleased: false);
        var manOfTomorrow = this.CreateMovie(
            "Man of Tomorrow", liveActionMovie, 2027, "tt37833661", "m/man_of_tomorrow", isReleased: false);

        godsAndMonsters.AttachSeries(creatureCommandos, true);
        godsAndMonsters.AttachMovie(superman, true);
        godsAndMonsters.AttachMovie(supergirl, true);
        godsAndMonsters.AttachSeries(lanterns, true);
        godsAndMonsters.AttachMovie(clayface, true);
        godsAndMonsters.AttachMovie(manOfTomorrow, true);

        list.AddFranchise(dc);
        list.AddFranchise(godsAndMonsters);
        list.AddSeries(creatureCommandos);
        list.AddMovie(superman);
        list.AddMovie(supergirl);
        list.AddSeries(lanterns);
        list.AddMovie(clayface);
        list.AddMovie(manOfTomorrow);

        list.SortItems();
    }

    private Movie CreateMovie(
        string title,
        MovieKind kind,
        int year,
        string imdbId,
        string rottenTomatoesId,
        bool isReleased = true) =>
        this.CreateMovie(title, title, kind, year, imdbId, rottenTomatoesId, isReleased);

    private Movie CreateMovie(
        string title,
        string originalTitle,
        MovieKind kind,
        int year,
        string imdbId,
        string rottenTomatoesId,
        bool isReleased = true) =>
        new(
            Id.Create<Movie>(),
            [new(title, 1, false), new(originalTitle, 1, true)],
            year,
            isWatched: isReleased,
            isReleased: isReleased,
            kind)
        {
            ImdbId = new ImdbId(imdbId),
            RottenTomatoesId = new RottenTomatoesId(rottenTomatoesId)
        };

    private LimitedSeries BandOfBrothers(SeriesKind kind)
    {
        const string title = "Band of Brothers";

        return new(
            Id.Create<LimitedSeries>(),
            [new(title, 1, false), new(title, 1, true)],
            new(Month.September, 2001, Month.November, 2001, false, 10),
            SeriesWatchStatus.Watched,
            SeriesReleaseStatus.Finished,
            "HBO",
            kind)
        {
            ImdbId = new ImdbId("tt0185906"),
            RottenTomatoesId = new RottenTomatoesId("tv/band_of_brothers"),
            RottenTomatoesSubId = new RottenTomatoesId("tv/band_of_brothers/s01")
        };
    }

    private Series DuneProphecy(SeriesKind kind)
    {
        const string title = "Dune: Prophecy";
        const string channel = "HBO";

        var part1 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.November, 2024, Month.December, 2024, false, 6))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/dune_prophecy/s01")
        };

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [part1]);

        var part2 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.November, 2026, Month.December, 2026, false, 6))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/dune_prophecy/s02")
        };

        var season2 = new Season(
            Id.Create<Season>(),
            [new("Season 2", 1, false), new("Season 2", 1, true)],
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.NotStarted,
            channel,
            sequenceNumber: 2,
            [part2]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1, season2],
            [],
            SeriesWatchStatus.Watching,
            SeriesReleaseStatus.Running,
            kind)
        {
            ImdbId = new ImdbId("tt10466872"),
            RottenTomatoesId = new RottenTomatoesId("tv/dune_prophecy")
        };
    }

    private Series MrRobot(SeriesKind kind)
    {
        const string title = "Mr. Robot";
        const string channel = "USA";

        var part1 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.June, 2015, Month.September, 2015, false, 10))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/mr_robot/s01")
        };

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [part1]);

        var part2 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.July, 2016, Month.September, 2016, false, 12))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/mr_robot/s02")
        };

        var season2 = new Season(
            Id.Create<Season>(),
            [new("Season 2", 1, false), new("Season 2", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 2,
            [part2]);

        var part3 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.October, 2017, Month.December, 2017, false, 10))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/mr_robot/s03")
        };

        var season3 = new Season(
            Id.Create<Season>(),
            [new("Season 3", 1, false), new("Season 3", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 3,
            [part3]);

        var part4 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.October, 2019, Month.December, 2019, false, 13))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/mr_robot/s04")
        };

        var season4 = new Season(
            Id.Create<Season>(),
            [new("Season 4", 1, false), new("Season 4", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 4,
            [part4]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1, season2, season3, season4],
            [],
            SeriesWatchStatus.Watched,
            SeriesReleaseStatus.Finished,
            kind)
        {
            ImdbId = new ImdbId("tt4158110"),
            RottenTomatoesId = new RottenTomatoesId("tv/mr_robot")
        };
    }

    private Series Sherlock(SeriesKind kind)
    {
        const string title = "Sherlock";
        const string channel = "BBC One";

        var part1 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.July, 2010, Month.August, 2010, false, 3))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/sherlock/s01")
        };

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [part1]);

        var part2 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.January, 2012, Month.January, 2012, false, 3))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/sherlock/s02")
        };

        var season2 = new Season(
            Id.Create<Season>(),
            [new("Season 2", 1, false), new("Season 2", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 2,
            [part2]);

        var part3 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.January, 2014, Month.January, 2014, false, 3))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/sherlock/s03")
        };

        var season3 = new Season(
            Id.Create<Season>(),
            [new("Season 3", 1, false), new("Season 3", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 3,
            [part3]);

        var episode = new SpecialEpisode(
            Id.Create<SpecialEpisode>(),
            [new("The Abominable Bride", 1, false), new("The Abominable Bride", 1, true)],
            month: Month.January,
            year: 2016,
            isWatched: true,
            isReleased: true,
            channel,
            sequenceNumber: 4)
        {
            RottenTomatoesId = new RottenTomatoesId("tv/sherlock/15886")
        };

        var part4 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.January, 2017, Month.January, 2017, false, 3))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/sherlock/s04")
        };

        var season4 = new Season(
            Id.Create<Season>(),
            [new("Season 4", 1, false), new("Season 4", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 5,
            [part4]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1, season2, season3, season4],
            [episode],
            SeriesWatchStatus.Watched,
            SeriesReleaseStatus.Finished,
            kind)
        {
            ImdbId = new ImdbId("tt1475582"),
            RottenTomatoesId = new RottenTomatoesId("tv/sherlock")
        };
    }

    private Series CreatureCommandos(SeriesKind kind)
    {
        const string title = "Creature Commandos";
        const string channel = "HBO";

        var part1 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.December, 2024, Month.January, 2025, false, 7))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/creature_commandos/s01")
        };

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [part1]);

        var part2 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.January, 2027, Month.February, 2027, false, 7))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/creature_commandos/s02")
        };

        var season2 = new Season(
            Id.Create<Season>(),
            [new("Season 2", 1, false), new("Season 2", 1, true)],
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.NotStarted,
            channel,
            sequenceNumber: 2,
            [part2]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1, season2],
            [],
            SeriesWatchStatus.Watching,
            SeriesReleaseStatus.Running,
            kind)
        {
            ImdbId = new ImdbId("tt26545355"),
            RottenTomatoesId = new RottenTomatoesId("tv/creature_commandos")
        };
    }

    private Series Lanterns(SeriesKind kind)
    {
        const string title = "Lanterns";
        const string channel = "HBO";

        var part1 = new SeasonPart(Id.Create<SeasonPart>(), new(Month.August, 2026, Month.October, 2026, false, 8))
        {
            RottenTomatoesId = new RottenTomatoesId("tv/lanterns/s01")
        };

        var season1 = new Season(
            Id.Create<Season>(),
            [new("Season 1", 1, false), new("Season 1", 1, true)],
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            [part1]);

        return new(
            Id.Create<Series>(),
            [new(title, 1, false), new(title, 1, true)],
            [season1],
            [],
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.NotStarted,
            kind)
        {
            ImdbId = new ImdbId("tt26545992"),
            RottenTomatoesId = new RottenTomatoesId("tv/lanterns")
        };
    }

    private Franchise CreateFranchise(
        string title,
        MovieKind movieKind,
        SeriesKind seriesKind,
        FranchiseKindSource kindSource,
        bool? showTitles = null,
        bool isLooselyConnected = false) =>
        new(
            Id.Create<Franchise>(),
            title is not null ? [new(title, 1, false), new(title, 1, true)] : new List<Title>(),
            movieKind,
            seriesKind,
            kindSource,
            showTitles: showTitles ?? title is not null,
            isLooselyConnected: isLooselyConnected,
            continueNumbering: false);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Error when registering a test user: {Error}",
        SkipEnabledCheck = true)]
    private partial void LogUserRegistrationError(IdentityError? error);
}
