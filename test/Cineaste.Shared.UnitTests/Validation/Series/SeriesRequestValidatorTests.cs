using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Validation.TestData;

using static Cineaste.Shared.Validation.TestData.TitleUtils;

namespace Cineaste.Shared.Validation.Series;

public class SeriesRequestValidatorTests
{
    private static readonly string[] SingleEmptyString = [""];

    public readonly SeriesRequestValidator validator = new();

    [Fact(DisplayName = "Validator should validate that titles aren't empty")]
    public void ValidatorShouldValidateTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(titles: []));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Series.Titles.Empty");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct names")]
    public void ValidatorShouldValidateTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(titles: [title.Get, title.Get]));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Series.Titles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct priorities")]
    public void ValidatorShouldValidateTitlesDistinctPriorities(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            titles: [title1.Get, title2.Get], differentTitlePriorities: false));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Series.Titles.Distinct.Priorities");
    }

    [Fact(DisplayName = "Validator should validate that original titles aren't empty")]
    public void ValidatorShouldValidateOriginalTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(originalTitles: []));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Series.OriginalTitles.Empty");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct names")]
    public void ValidatorShouldValidateOriginalTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(originalTitles: [title.Get, title.Get]));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Series.OriginalTitles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct priorities")]
    public void ValidatorShouldValidateOriginalTitlesDistinctPriorities(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            originalTitles: [title1.Get, title2.Get], differentOriginalTitlePriorities: false));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Series.OriginalTitles.Distinct.Priorities");
    }

    [Fact(DisplayName = "Validator should validate titles")]
    public void ValidatorShouldValidateTitles()
    {
        var result = validator.TestValidate(this.Request(titles: SingleEmptyString));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("Titles.Name.Empty");
    }

    [Fact(DisplayName = "Validator should validate original titles")]
    public void ValidatorShouldValidateOriginalTitles()
    {
        var result = validator.TestValidate(this.Request(originalTitles: SingleEmptyString));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("OriginalTitles.Name.Empty");
    }

    [Property(DisplayName = "Validator should validate watch status")]
    public void ValidatorShouldValidateWatchStatus(int watchStatus)
    {
        var result = validator.TestValidate(this.Request(watchStatus: (SeriesWatchStatus)watchStatus));

        if (Enum.IsDefined(typeof(SeriesWatchStatus), watchStatus))
        {
            result.ShouldNotHaveValidationErrorFor(req => req.WatchStatus);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.WatchStatus)
                .WithErrorCode("Series.WatchStatus.Invalid");
        }
    }

    [Property(DisplayName = "Validator should validate release status")]
    public void ValidatorShouldValidateReleaseStatus(int releaseStatus)
    {
        var result = validator.TestValidate(this.Request(releaseStatus: (SeriesReleaseStatus)releaseStatus));

        if (Enum.IsDefined(typeof(SeriesReleaseStatus), releaseStatus))
        {
            result.ShouldNotHaveValidationErrorFor(req => req.ReleaseStatus);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.ReleaseStatus)
                .WithErrorCode("Series.ReleaseStatus.Invalid");
        }
    }

    [ClassData(typeof(SeriesTestData))]
    [Theory(DisplayName = "Validator should validate correlation of watch and release status")]
    public void ValidatorShouldValidateCorrelationOfWatchAndReleaseStatus(
        SeriesWatchStatus watchStatus,
        SeriesReleaseStatus releaseStatus,
        bool isValid)
    {
        var result = validator.TestValidate(this.Request(watchStatus: watchStatus, releaseStatus: releaseStatus));

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        } else
        {
            result.ShouldHaveValidationErrors()
                .WithErrorCode("Series.WatchStatus.Invalid");
        }
    }

    [Fact(DisplayName = "Validator should validate that seasons aren't empty")]
    public void ValidatorShouldValidatePeriodssNotEmpty()
    {
        var result = validator.TestValidate(this.Request(seasons: []));

        result.ShouldHaveValidationErrorFor(req => req.Seasons)
            .WithErrorCode("Series.Seasons.Empty");
    }

    [Fact(DisplayName = "Validator should validate that seasons and special episode have correct sequence")]
    public void ValidatorShouldValidateCorrectSequence()
    {
        var seasons = Enumerable.Range(1, 9)
            .Where(num => num % 2 == 1)
            .Select(this.Season);

        var episodes = Enumerable.Range(1, 9)
            .Where(num => num % 2 == 0)
            .Select(this.SpecialEpisode);

        var result1 = validator.TestValidate(this.Request(seasons: seasons, specialEpisodes: episodes));
        var result2 = validator.TestValidate(this.Request(seasons: seasons.Take(2), specialEpisodes: episodes));

        result1.ShouldNotHaveAnyValidationErrors();
        result2.ShouldHaveValidationErrors()
            .WithErrorCode("Series.Sequence.Invalid");
    }

    [Fact(DisplayName = "Validator should validate seasons")]
    public void ValidatorShouldValidateSeasons()
    {
        var result = validator.TestValidate(this.Request(seasons:
        [
            new SeasonRequest(
                null,
                TitleRequests("Test"),
                TitleRequests("Test"),
                1,
                SeasonWatchStatus.NotWatched,
                SeasonReleaseStatus.Finished,
                "",
                ImmutableList.Create(new PeriodRequest(null, 1, 2000, 2, 2000, 5, false, null)).AsValue())
        ]));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("Season.Channel.Empty");
    }

    [Fact(DisplayName = "Validator should validate specialEpisodes")]
    public void ValidatorShouldValidateSpecialEpisodes()
    {
        var result = validator.TestValidate(this.Request(specialEpisodes:
        [
            new SpecialEpisodeRequest(
                null,
                TitleRequests("Test"),
                TitleRequests("Test"),
                1,
                false,
                true,
                "",
                1,
                2000,
                null)
        ]));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("SpecialEpisode.Channel.Empty");
    }

    [ClassData(typeof(ImdbIdTestData))]
    [Theory(DisplayName = "Validator should validate IMDb ID")]
    public void ValidatorShouldValidateImdbId(string? imdbId, bool isValid)
    {
        var result = validator.TestValidate(this.Request(imdbId: imdbId));

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.ImdbId);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.ImdbId)
                .WithErrorCode("Series.ImdbId.Invalid");
        }
    }

    [ClassData(typeof(RottenTomatoesIdTestData))]
    [Theory(DisplayName = "Validator should validate Rotten Tomatoes ID")]
    public void ValidatorShouldValidateRottenTomatoesId(string? rottenTomatoesId, bool isValid)
    {
        var result = validator.TestValidate(this.Request(rottenTomatoesId: rottenTomatoesId));

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.RottenTomatoesId);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.RottenTomatoesId)
                .WithErrorCode("Series.RottenTomatoesId.Invalid");
        }
    }

    private SeriesRequest Request(
        IEnumerable<string>? titles = null,
        bool differentTitlePriorities = true,
        IEnumerable<string>? originalTitles = null,
        bool differentOriginalTitlePriorities = true,
        SeriesWatchStatus watchStatus = SeriesWatchStatus.NotWatched,
        SeriesReleaseStatus releaseStatus = SeriesReleaseStatus.Finished,
        IEnumerable<SeasonRequest>? seasons = null,
        IEnumerable<SpecialEpisodeRequest>? specialEpisodes = null,
        string? imdbId = null,
        string? rottenTomatoesId = null) =>
        new(
            Guid.CreateVersion7(),
            TitleRequests(titles, differentTitlePriorities),
            TitleRequests(originalTitles, differentOriginalTitlePriorities),
            watchStatus,
            releaseStatus,
            Guid.CreateVersion7(),
            seasons?.ToImmutableList().AsValue() ?? this.DefaultSeasons(),
            specialEpisodes?.ToImmutableList().AsValue() ?? ImmutableList.Create<SpecialEpisodeRequest>().AsValue(),
            imdbId,
            rottenTomatoesId);

    private ImmutableValueList<SeasonRequest> DefaultSeasons() =>
        ImmutableList.Create(this.Season(1)).AsValue();

    private SeasonRequest Season(int sequenceNumber) =>
        new(
            null,
            TitleRequests("Test"),
            TitleRequests("Test"),
            sequenceNumber,
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.Finished,
            "Test",
            ImmutableList.Create(new PeriodRequest(null, 1, 2000, 2, 2000, 5, false, null)).AsValue());

    private SpecialEpisodeRequest SpecialEpisode(int sequenceNumber) =>
        new(null, TitleRequests("Test"), TitleRequests("Test"), sequenceNumber, false, true, "Test", 1, 2000, null);
}
