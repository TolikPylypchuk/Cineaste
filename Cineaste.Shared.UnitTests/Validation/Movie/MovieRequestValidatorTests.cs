namespace Cineaste.Shared.Validation.Movie;

using Cineaste.Shared.Models.Movie;
using Cineaste.Shared.Validation.TestData;

using FsCheck;

using static Cineaste.Shared.Validation.TestData.TitleUtils;

public sealed class MovieRequestValidatorTests
{
    private static readonly string[] SingleEmptyString = [""];

    private readonly MovieRequestValidator validator = new();

    public static Arbitrary<int> ValidYear =>
        new ArbitraryValidYear();

    [Fact(DisplayName = "Validator should validate that titles aren't empty")]
    public void ValidatorShouldValidateTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(titles: Enumerable.Empty<string>()));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Movie.Titles.Empty");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct names")]
    public void ValidatorShouldValidateTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(titles: new[] { title.Get, title.Get }));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Movie.Titles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct priorities")]
    public void ValidatorShouldValidateTitlesDistinctPriorities(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            titles: new[] { title1.Get, title2.Get }, differentTitlePriorities: false));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Movie.Titles.Distinct.Priorities");
    }

    [Fact(DisplayName = "Validator should validate that original titles aren't empty")]
    public void ValidatorShouldValidateOriginalTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(originalTitles: Enumerable.Empty<string>()));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Movie.OriginalTitles.Empty");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct names")]
    public void ValidatorShouldValidateOriginalTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(originalTitles: new[] { title.Get, title.Get }));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Movie.OriginalTitles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct priorities")]
    public void ValidatorShouldValidateOriginalTitlesDistinctPriorities(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            originalTitles: new[] { title1.Get, title2.Get }, differentOriginalTitlePriorities: false));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Movie.OriginalTitles.Distinct.Priorities");
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

    [Property(DisplayName = "Validator should validate correlation of watched and released")]
    public void ValidatorShouldValidateCorrelationOfWatchedAndReleased(bool isWatched, bool isReleased)
    {
        var result = validator.TestValidate(this.Request(isWatched: isWatched, isReleased: isReleased));

        if (isWatched && !isReleased)
        {
            result.ShouldHaveValidationErrors()
                .WithErrorCode("Movie.IsWatched.Invalid");
        } else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Property(
        DisplayName = "Validator should validate correlation of year and released",
        Arbitrary = new[] { typeof(MovieRequestValidatorTests) })]
    public void ValidatorShouldValidateCorrelationOfYearAndReleased(int year, bool isReleased)
    {
        int thisYear = DateTime.Now.Year;

        var result = validator.TestValidate(this.Request(year: year, isReleased: isReleased));

        if (year < thisYear && !isReleased || thisYear < year && isReleased)
        {
            result.ShouldHaveValidationErrors()
                .WithErrorCode("Movie.IsReleased.Invalid");
        } else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Property(DisplayName = "Validator should validate year")]
    public void ValidatorShouldValidateYear(int year)
    {
        var result = validator.TestValidate(this.Request(year: year));

        if (year > MinYear)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Year);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.Year)
                .WithErrorCode("Movie.Year.TooLow");
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
                .WithErrorCode("Movie.ImdbId.Invalid");
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
                .WithErrorCode("Movie.RottenTomatoesId.Invalid");
        }
    }

    private MovieRequest Request(
        IEnumerable<string>? titles = null,
        bool differentTitlePriorities = true,
        IEnumerable<string>? originalTitles = null,
        bool differentOriginalTitlePriorities = true,
        int? year = null,
        bool isWatched = false,
        bool isReleased = true,
        string? imdbId = null,
        string? rottenTomatoesId = null) =>
        new(
            Guid.NewGuid(),
            TitleRequests(titles, differentTitlePriorities),
            TitleRequests(originalTitles, differentOriginalTitlePriorities),
            year ?? DateTime.Now.Year,
            isWatched,
            isReleased,
            Guid.NewGuid(),
            imdbId,
            rottenTomatoesId);
}
