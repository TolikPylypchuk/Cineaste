namespace Cineaste.Shared.Validation.Series;

using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Validation.TestData;

using FluentValidation.TestHelper;

using FsCheck;

public class PeriodRequestValidatorTests
{
    private readonly PeriodRequestValidator validator = new();

    public static Arbitrary<int> ValidYear =>
        new ArbitraryValidYear();

    public static Arbitrary<byte> ValidMonth =>
        new ArbitraryValidMonth();

    [Property(DisplayName = "Validator should validate start month")]
    public void ValidatorShouldValidateStartMonth(int month)
    {
        var result = validator.TestValidate(this.Request(startMonth: month));

        if (1 <= month && month <= 12)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.StartMonth);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.StartMonth)
                .WithErrorCode("Period.StartMonth.Invalid");
        }
    }

    [Property(DisplayName = "Validator should validate start year")]
    public void ValidatorShouldValidateStartYear(int year)
    {
        var result = validator.TestValidate(this.Request(startYear: year));

        if (year >= MinYear)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.StartYear);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.StartYear)
                .WithErrorCode("Period.StartYear.TooLow");
        }
    }

    [Property(DisplayName = "Validator should validate end month")]
    public void ValidatorShouldValidateEndMonth(int month)
    {
        var result = validator.TestValidate(this.Request(endMonth: month));

        if (1 <= month && month <= 12)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.EndMonth);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.EndMonth)
                .WithErrorCode("Period.EndMonth.Invalid");
        }
    }

    [Property(DisplayName = "Validator should validate end year")]
    public void ValidatorShouldValidateEndYear(int year)
    {
        var result = validator.TestValidate(this.Request(endYear: year));

        if (year >= MinYear)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.EndYear);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.EndYear)
                .WithErrorCode("Period.EndYear.TooLow");
        }
    }

    [Property(
        DisplayName = "Validator should validate the period itself",
        Arbitrary = new[] { typeof(PeriodRequestValidatorTests) })]
    public void ValidatorShouldValidatePeriod(int startYear, int endYear, byte startMonth, byte endMonth)
    {
        var result = validator.TestValidate(this.Request(
            startMonth: startMonth, startYear: startYear, endMonth: endMonth, endYear: endYear));

        if (startYear < endYear || startYear == endYear && startMonth <= endMonth)
        {
            result.ShouldNotHaveAnyValidationErrors();
        } else
        {
            result.ShouldHaveAnyValidationError()
                .WithErrorCode("Period.Invalid");
        }
    }

    [Property(
        DisplayName = "Validator should validate the period itself",
        Arbitrary = new[] { typeof(PeriodRequestValidatorTests) })]
    public void ValidatorShouldValidateSingleDayRelease(
        int year1,
        int year2,
        byte month1,
        byte month2,
        bool isSingleDayRelease)
    {
        int startYear = Math.Min(year1, year2);
        int endYear = Math.Max(year1, year2);

        byte startMonth = Math.Min(month1, month2);
        byte endMonth = Math.Max(month1, month2);

        var result = validator.TestValidate(this.Request(
            startMonth: startMonth,
            startYear: startYear,
            endMonth: endMonth,
            endYear: endYear,
            isSingleDayRelease: isSingleDayRelease));

        if (isSingleDayRelease && (startYear != endYear || startMonth != endMonth))
        {
            result.ShouldHaveAnyValidationError()
                .WithErrorCode("Period.IsSingleDayRelease.Invalid");
        } else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Property(DisplayName = "Validator should validate episode count")]
    public void ValidatorShouldValidateEpisodeCount(int count)
    {
        var result = validator.TestValidate(this.Request(episodeCount: count));

        if (1 <= count && count <= 100)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.EpisodeCount);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.EpisodeCount)
                .WithErrorCode("Period.EpisodeCount.Invalid");
        }
    }

    [Property(DisplayName = "Validator should validate correlation of episode count and single-day release")]
    public void ValidatorShouldValidateCorrelationOfEpisodeCountAndSingleDayRelease(PositiveInt count)
    {
        var result = validator.TestValidate(this.Request(episodeCount: count.Get, isSingleDayRelease: false));

        if (count.Get != 1)
        {
            result.ShouldNotHaveAnyValidationErrors();
        } else
        {
            result.ShouldHaveAnyValidationError()
                .WithErrorCode("Period.IsSingleDayRelease.MustBeTrue");
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
                .WithErrorCode("Period.RottenTomatoesId.Invalid");
        }
    }

    private PeriodRequest Request(
        int startMonth = 1,
        int startYear = 2000,
        int endMonth = 1,
        int endYear = 2001,
        int episodeCount = 20,
        bool isSingleDayRelease = false,
        string? rottenTomatoesId = null) =>
        new(null, startMonth, startYear, endMonth, endYear, episodeCount, isSingleDayRelease, rottenTomatoesId);
}
