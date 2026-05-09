using Cineaste.Shared.Models.Movie;
using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Validation.TestData;

using static Cineaste.Shared.Validation.TestData.TitleUtils;

namespace Cineaste.Shared.Validation.Series;

public class LimitedSeriesRequestValidatorTests
{
    private static readonly string[] SingleEmptyString = [""];

    private readonly LimitedSeriesRequestValidator validator = new();

    public static Arbitrary<int> ValidYear =>
        new ArbitraryValidYear();

    public static Arbitrary<byte> ValidMonth =>
        new ArbitraryValidMonth();

    [Fact(DisplayName = "Validator should validate that titles aren't empty")]
    public void ValidatorShouldValidateTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(titles: []));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("LimitedSeries.Titles.Empty");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct names")]
    public void ValidatorShouldValidateTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(titles: [title.Get, title.Get]));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("LimitedSeries.Titles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct sequence numbers")]
    public void ValidatorShouldValidateTitlesDistinctSequenceNumbers(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            titles: [title1.Get, title2.Get], differentTitleSequenceNumbers: false));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("LimitedSeries.Titles.Distinct.SequenceNumbers");
    }

    [Fact(DisplayName = "Validator should validate that original titles aren't empty")]
    public void ValidatorShouldValidateOriginalTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(originalTitles: []));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("LimitedSeries.OriginalTitles.Empty");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct names")]
    public void ValidatorShouldValidateOriginalTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(originalTitles: [title.Get, title.Get]));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("LimitedSeries.OriginalTitles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct sequence numbers")]
    public void ValidatorShouldValidateOriginalTitlesDistinctSequenceNumbers(
        NonEmptyString title1,
        NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            originalTitles: [title1.Get, title2.Get], differentOriginalTitleSequenceNumbers: false));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("LimitedSeries.OriginalTitles.Distinct.SequenceNumbers");
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

    [Fact(DisplayName = "Validator should validate the period")]
    public void ValidatorShouldValidatePeriod()
    {
        var result = validator.TestValidate(this.Request(startYear: 2000, endYear: 1999));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("ReleasePeriod.Invalid");
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
                .WithErrorCode("LimitedSeries.WatchStatus.Invalid");
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
                .WithErrorCode("LimitedSeries.ReleaseStatus.Invalid");
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
                .WithErrorCode("LimitedSeries.WatchStatus.Invalid");
        }
    }

    [Property(DisplayName = "Validator should validate channel")]
    public void ValidatorShouldValidateChannel(string channel)
    {
        var result = validator.TestValidate(this.Request(channel: channel));

        if (String.IsNullOrWhiteSpace(channel))
        {
            result.ShouldHaveValidationErrorFor(req => req.Channel)
                .WithErrorCode("LimitedSeries.Channel.Empty");
        } else if (channel.Length > MaxNameLength)
        {
            result.ShouldHaveValidationErrorFor(req => req.Channel)
                .WithErrorCode("LimitedSeries.Channel.TooLong");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Channel);
        }
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
                .WithErrorCode("LimitedSeries.ImdbId.Invalid");
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
                .WithErrorCode("LimitedSeries.RottenTomatoesId.Invalid");
        }
    }

    [ClassData(typeof(RottenTomatoesIdTestData))]
    [Theory(DisplayName = "Validator should validate Rotten Tomatoes sub-ID")]
    public void ValidatorShouldValidateRottenTomatoesSubId(string? rottenTomatoesSubId, bool isValid)
    {
        var result = validator.TestValidate(this.Request(rottenTomatoesSubId: rottenTomatoesSubId));

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.RottenTomatoesSubId);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.RottenTomatoesSubId)
                .WithErrorCode("LimitedSeries.RottenTomatoesSubId.Invalid");
        }
    }

    private LimitedSeriesRequest Request(
        IEnumerable<string>? titles = null,
        bool differentTitleSequenceNumbers = true,
        IEnumerable<string>? originalTitles = null,
        bool differentOriginalTitleSequenceNumbers = true,
        SeriesWatchStatus watchStatus = SeriesWatchStatus.NotWatched,
        SeriesReleaseStatus releaseStatus = SeriesReleaseStatus.NotStarted,
        int startMonth = 1,
        int startYear = 2000,
        int endMonth = 1,
        int endYear = 2001,
        int episodeCount = 20,
        bool isSingleDayRelease = false,
        string? channel = "Test",
        string? imdbId = null,
        string? rottenTomatoesId = null,
        string? rottenTomatoesSubId = null) =>
        new(
            TitleRequests(titles, differentTitleSequenceNumbers),
            TitleRequests(originalTitles, differentOriginalTitleSequenceNumbers),
            new(startMonth, startYear, endMonth, endYear, episodeCount, isSingleDayRelease),
            watchStatus,
            releaseStatus,
            channel!,
            Guid.CreateVersion7(),
            imdbId,
            rottenTomatoesId,
            rottenTomatoesSubId,
            null);
}
