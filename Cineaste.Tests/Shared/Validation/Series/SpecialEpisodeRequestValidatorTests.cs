namespace Cineaste.Shared.Validation.Series;

using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Validation.TestData;

using FsCheck;

using static Cineaste.Shared.Validation.TestData.TitleUtils;

public class SpecialEpisodeRequestValidatorTests
{
    private readonly SpecialEpisodeRequestValidator validator = new();

    public static Arbitrary<int> ValidYear =>
        new ArbitraryValidYear();

    public static Arbitrary<byte> ValidMonth =>
        new ArbitraryValidMonth();

    [Fact(DisplayName = "Validator should validate that titles aren't empty")]
    public void ValidatorShouldValidateTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(titles: Enumerable.Empty<string>()));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("SpecialEpisode.Titles.Empty");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct names")]
    public void ValidatorShouldValidateTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(titles: new[] { title.Get, title.Get }));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("SpecialEpisode.Titles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct priorities")]
    public void ValidatorShouldValidateTitlesDistinctPriorities(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            titles: new[] { title1.Get, title2.Get }, differentTitlePriorities: false));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("SpecialEpisode.Titles.Distinct.Priorities");
    }

    [Fact(DisplayName = "Validator should validate that original titles aren't empty")]
    public void ValidatorShouldValidateOriginalTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(originalTitles: Enumerable.Empty<string>()));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("SpecialEpisode.OriginalTitles.Empty");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct names")]
    public void ValidatorShouldValidateOriginalTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(originalTitles: new[] { title.Get, title.Get }));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("SpecialEpisode.OriginalTitles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct priorities")]
    public void ValidatorShouldValidateOriginalTitlesDistinctPriorities(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            originalTitles: new[] { title1.Get, title2.Get }, differentOriginalTitlePriorities: false));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("SpecialEpisode.OriginalTitles.Distinct.Priorities");
    }

    [Property(DisplayName = "Validator should validate correlation of watched and released")]
    public void ValidatorShouldValidateCorrelationOfWatchedAndReleased(bool isWatched, bool isReleased)
    {
        var result = validator.TestValidate(this.Request(isWatched: isWatched, isReleased: isReleased));

        if (isWatched && !isReleased)
        {
            result.ShouldHaveAnyValidationError()
                .WithErrorCode("SpecialEpisode.IsWatched.Invalid");
        } else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Property(DisplayName = "Validator should validate channel")]
    public void ValidatorShouldValidateName(string channel)
    {
        var result = validator.TestValidate(this.Request(channel: channel));

        if (String.IsNullOrWhiteSpace(channel))
        {
            result.ShouldHaveValidationErrorFor(req => req.Channel)
                .WithErrorCode("SpecialEpisode.Channel.Empty");
        } else if (channel.Length > MaxNameLength)
        {
            result.ShouldHaveValidationErrorFor(req => req.Channel)
                .WithErrorCode("SpecialEpisode.Channel.TooLong");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Channel);
        }
    }

    [Property(
        DisplayName = "Validator should validate correlation of month/year and released",
        Arbitrary = new[] { typeof(SpecialEpisodeRequestValidatorTests) })]
    public void ValidatorShouldValidateCorrelationOfMonthYearAndReleased(byte month, int year, bool isReleased)
    {
        int thisYear = DateTime.Now.Year;
        int thisMonth = DateTime.Now.Year;

        var result = validator.TestValidate(this.Request(month: month, year: year, isReleased: isReleased));

        if (year < thisYear && !isReleased ||
            year == thisYear && month < thisMonth && !isReleased ||
            thisYear < year && isReleased ||
            year == thisYear && month > thisMonth && isReleased)
        {
            result.ShouldHaveAnyValidationError()
                .WithErrorCode("SpecialEpisode.IsReleased.Invalid");
        } else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Property(DisplayName = "Validator should validate month")]
    public void ValidatorShouldValidateMonth(int month)
    {
        var result = validator.TestValidate(this.Request(month: month));

        if (1 <= month && month <= 12)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Month);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.Month)
                .WithErrorCode("SpecialEpisode.Month.Invalid");
        }
    }

    [Property(DisplayName = "Validator should validate year")]
    public void ValidatorShouldValidateYear(int year)
    {
        var result = validator.TestValidate(this.Request(year: year));

        if (year >= MinYear)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Year);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.Year)
                .WithErrorCode("SpecialEpisode.Year.TooLow");
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
                .WithErrorCode("SpecialEpisode.RottenTomatoesId.Invalid");
        }
    }

    private SpecialEpisodeRequest Request(
        IEnumerable<string>? titles = null,
        bool differentTitlePriorities = true,
        IEnumerable<string>? originalTitles = null,
        bool differentOriginalTitlePriorities = true,
        int sequenceNumber = 1,
        bool isWatched = false,
        bool isReleased = true,
        string channel = "Test",
        int? month = null,
        int? year = null,
        string? rottenTomatoesId = null) =>
        new(
            null,
            TitleRequests(titles, differentTitlePriorities),
            TitleRequests(originalTitles, differentOriginalTitlePriorities),
            sequenceNumber,
            isWatched,
            isReleased,
            channel,
            month ?? DateTime.Now.Month,
            year ?? DateTime.Now.Year,
            rottenTomatoesId);
}
