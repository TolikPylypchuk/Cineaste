using Cineaste.Shared.Models.Series;
using Cineaste.Shared.Validation.TestData;

namespace Cineaste.Shared.Validation.Series;

public class SeasonPartRequestValidatorTests
{
    private readonly SeasonPartRequestValidator validator = new();

    public static Arbitrary<int> ValidYear =>
        new ArbitraryValidYear();

    public static Arbitrary<byte> ValidMonth =>
        new ArbitraryValidMonth();

    [Fact(DisplayName = "Validator should validate periods")]
    public void ValidatorShouldValidateParts()
    {
        var result = validator.TestValidate(this.Request(1, 2000, 2, 1999, 5, false, null));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("ReleasePeriod.Invalid");
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
                .WithErrorCode("SeasonPart.RottenTomatoesId.Invalid");
        }
    }

    private SeasonPartRequest Request(
        int startMonth = 1,
        int startYear = 2000,
        int endMonth = 1,
        int endYear = 2001,
        int episodeCount = 20,
        bool isSingleDayRelease = false,
        string? rottenTomatoesId = null) =>
        new(null, new(startMonth, startYear, endMonth, endYear, episodeCount, isSingleDayRelease), rottenTomatoesId);
}
