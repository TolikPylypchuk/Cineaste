using Cineaste.Shared.Models.Poster;

namespace Cineaste.Shared.Validation.Poster;

public sealed class PosterImdbMediaRequestValidatorTests
{
    private readonly PosterImdbMediaRequestValidator validator = new();

    [Fact(DisplayName = "Validator should validate that the URL isn't null")]
    public void ValidatorShouldValidateUrlNotNull()
    {
        var result = validator.TestValidate(new PosterImdbMediaRequest(null!));

        result.ShouldHaveValidationErrorFor(req => req.Url)
            .WithErrorCode("Poster.ImdbMedia.Url.Empty");
    }

    [Fact(DisplayName = "Validator should validate that the URL isn't empty")]
    public void ValidatorShouldValidateTitlesNotEmpty()
    {
        var result = validator.TestValidate(new PosterImdbMediaRequest(String.Empty));

        result.ShouldHaveValidationErrorFor(req => req.Url)
            .WithErrorCode("Poster.ImdbMedia.Url.Empty");
    }

    [MemberData(nameof(ValidUrlsTestData))]
    [Theory(DisplayName = "Validator should validate the URL")]
    public void ValidatorShouldValidateUrl(string url, bool isValid, bool hasNoPath)
    {
        var result = validator.TestValidate(new PosterImdbMediaRequest(url));

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Url);
        } else if (hasNoPath)
        {
            result.ShouldHaveValidationErrorFor(req => req.Url)
                .WithErrorCode("Poster.ImdbMedia.Url.NoPath");
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.Url)
                .WithErrorCode("Poster.ImdbMedia.Url.Invalid");
        }
    }

    public static IEnumerable<TheoryDataRow<string, bool, bool>> ValidUrlsTestData()
    {
        // string url, bool isValid, bool hasNoPath
        yield return new("123qwe", false, false);
        yield return new("www.tolik.io", false, false);
        yield return new("http://www.tolik.io", false, true);
        yield return new("http://www.tolik.io/some-file.png", true, false);
        yield return new("https://www.tolik.io/abcd", true, false);
    }
}
