using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Validation.TestData;

using static Cineaste.Shared.Validation.TestData.TitleUtils;

namespace Cineaste.Shared.Validation.Series;

public class SeasonRequestValidatorTests
{
    private static readonly string[] SingleEmptyString = [""];

    private readonly SeasonRequestValidator validator = new();

    public static Arbitrary<PeriodRequest> ValidPeriodRequest =>
        new ArbitraryValidPeriodRequest();

    [Fact(DisplayName = "Validator should validate that titles aren't empty")]
    public void ValidatorShouldValidateTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(titles: []));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Season.Titles.Empty");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct names")]
    public void ValidatorShouldValidateTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(titles: [title.Get, title.Get]));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Season.Titles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct sequence numbers")]
    public void ValidatorShouldValidateTitlesDistinctSequenceNumbers(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            titles: [title1.Get, title2.Get], differentTitleSequenceNumbers: false));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Season.Titles.Distinct.SequenceNumbers");
    }

    [Fact(DisplayName = "Validator should validate that original titles aren't empty")]
    public void ValidatorShouldValidateOriginalTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(originalTitles: []));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Season.OriginalTitles.Empty");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct names")]
    public void ValidatorShouldValidateOriginalTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(originalTitles: [title.Get, title.Get]));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Season.OriginalTitles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct sequence numbers")]
    public void ValidatorShouldValidateOriginalTitlesDistinctSequenceNumbers(
        NonEmptyString title1,
        NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            originalTitles: [title1.Get, title2.Get], differentOriginalTitleSequenceNumbers: false));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Season.OriginalTitles.Distinct.SequenceNumbers");
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
        var result = validator.TestValidate(this.Request(watchStatus: (SeasonWatchStatus)watchStatus));

        if (Enum.IsDefined(typeof(SeasonWatchStatus), watchStatus))
        {
            result.ShouldNotHaveValidationErrorFor(req => req.WatchStatus);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.WatchStatus)
                .WithErrorCode("Season.WatchStatus.Invalid");
        }
    }

    [Property(DisplayName = "Validator should validate release status")]
    public void ValidatorShouldValidateReleaseStatus(int releaseStatus)
    {
        var result = validator.TestValidate(this.Request(releaseStatus: (SeasonReleaseStatus)releaseStatus));

        if (Enum.IsDefined(typeof(SeasonReleaseStatus), releaseStatus))
        {
            result.ShouldNotHaveValidationErrorFor(req => req.ReleaseStatus);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.ReleaseStatus)
                .WithErrorCode("Season.ReleaseStatus.Invalid");
        }
    }

    [ClassData(typeof(SeasonTestData))]
    [Theory(DisplayName = "Validator should validate correlation of watch and release status")]
    public void ValidatorShouldValidateCorrelationOfWatchAndReleaseStatus(
        SeasonWatchStatus watchStatus,
        SeasonReleaseStatus releaseStatus,
        bool isValid)
    {
        var result = validator.TestValidate(this.Request(watchStatus: watchStatus, releaseStatus: releaseStatus));

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        } else
        {
            result.ShouldHaveValidationErrors()
                .WithErrorCode("Season.WatchStatus.Invalid");
        }
    }

    [Property(DisplayName = "Validator should validate channel")]
    public void ValidatorShouldValidateChannel(string channel)
    {
        var result = validator.TestValidate(this.Request(channel: channel));

        if (String.IsNullOrWhiteSpace(channel))
        {
            result.ShouldHaveValidationErrorFor(req => req.Channel)
                .WithErrorCode("Season.Channel.Empty");
        } else if (channel.Length > MaxNameLength)
        {
            result.ShouldHaveValidationErrorFor(req => req.Channel)
                .WithErrorCode("Season.Channel.TooLong");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Channel);
        }
    }

    [Fact(DisplayName = "Validator should validate that periods aren't empty")]
    public void ValidatorShouldValidatePeriodssNotEmpty()
    {
        var result = validator.TestValidate(this.Request(periods: []));

        result.ShouldHaveValidationErrorFor(req => req.Periods)
            .WithErrorCode("Season.Periods.Empty");
    }

    [Property(
        DisplayName = "Validator should validate that periods don't overlap",
        Arbitrary = new[] { typeof(SeasonRequestValidatorTests) })]
    public void ValidatorShouldValidateThatPeriodsDoNotOverlap(PeriodRequest period1, PeriodRequest period2)
    {
        var result = validator.TestValidate(this.Request(periods: [period1, period2]));

        if (period1.EndYear < period2.StartYear ||
            period1.EndYear == period2.StartYear && period1.EndMonth <= period2.StartMonth ||
            period2.EndYear < period1.StartYear ||
            period2.EndYear == period1.StartYear && period2.EndMonth <= period1.StartMonth)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Periods);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.Periods)
                .WithErrorCode("Season.Periods.Overlap");
        }
    }

    [Fact(DisplayName = "Validator should validate periods")]
    public void ValidatorShouldValidatePeriods()
    {
        var result = validator.TestValidate(this.Request(
            periods: [new PeriodRequest(null, 1, 2000, 2, 1999, 5, false, null)]));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("Period.Invalid");
    }

    private SeasonRequest Request(
        IEnumerable<string>? titles = null,
        bool differentTitleSequenceNumbers = true,
        IEnumerable<string>? originalTitles = null,
        bool differentOriginalTitleSequenceNumbers = true,
        int sequenceNumber = 1,
        SeasonWatchStatus watchStatus = SeasonWatchStatus.NotWatched,
        SeasonReleaseStatus releaseStatus = SeasonReleaseStatus.Finished,
        string channel = "Test",
        IEnumerable<PeriodRequest>? periods = null
        ) =>
        new(
            null,
            TitleRequests(titles, differentTitleSequenceNumbers),
            TitleRequests(originalTitles, differentOriginalTitleSequenceNumbers),
            sequenceNumber,
            watchStatus,
            releaseStatus,
            channel,
            periods?.ToImmutableList().AsValue() ?? this.DefaultPeriods());

    private ImmutableValueList<PeriodRequest> DefaultPeriods() =>
        ImmutableList.Create(new PeriodRequest(null, 1, 2000, 2, 2000, 5, false, null)).AsValue();
}
