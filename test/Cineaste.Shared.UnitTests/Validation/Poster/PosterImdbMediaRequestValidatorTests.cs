using Cineaste.Shared.Models.Poster;
using Cineaste.Shared.Validation.TestData;

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

    [ClassData(typeof(UrlTestData))]
    [Theory(DisplayName = "Validator should validate the URL")]
    public void ValidatorShouldValidateUrl(string url, bool isValid)
    {
        var result = validator.TestValidate(new PosterImdbMediaRequest(url));

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Url);
        } else
        {
            result.ShouldHaveValidationErrorFor(req => req.Url)
                .WithErrorCode("Poster.ImdbMedia.Url.Invalid");
        }
    }
}
