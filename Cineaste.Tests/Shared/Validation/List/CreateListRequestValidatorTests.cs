namespace Cineaste.Shared.Validation.List;

using System.Collections.Immutable;
using System.Globalization;

using Models.List;

public sealed class CreateListRequestValidatorTests
{
    public static readonly IEnumerable<object[]> AvailableCultureData;
    private static readonly ImmutableHashSet<string> AvailableCultures;

    private const string Placeholder = "Test";

    private readonly CreateListRequestValidator validator;

    static CreateListRequestValidatorTests()
    {
        AvailableCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Select(culture => culture.ToString())
            .ToImmutableHashSet();

        AvailableCultureData = AvailableCultures.Select(culture => new[] { culture });
    }

    public CreateListRequestValidatorTests() =>
        this.validator = new();

    [Property(DisplayName = "Validator should validate name")]
    public void ValidatorShouldValidateName(string name)
    {
        var result = validator.TestValidate(this.Request(name: name));

        if (String.IsNullOrWhiteSpace(name))
        {
            result.ShouldHaveValidationErrorFor(req => req.Name)
                .WithErrorCode("CreateList.Name.Empty");
        } else if (name.Length > MaxNameLength)
        {
            result.ShouldHaveValidationErrorFor(req => req.Name)
                .WithErrorCode("CreateList.Name.TooLong");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Name);
        }
    }

    [Theory(DisplayName = "Validator should validate correct culture")]
    [MemberData(nameof(AvailableCultureData))]
    public void ValidatorShouldValidateCorrectCulture(string culture)
    {
        var result = validator.TestValidate(this.Request(culture: culture));
        result.ShouldNotHaveValidationErrorFor(req => req.Culture);
    }

    [Property(DisplayName = "Validator should validate incorrect culture")]
    public void ValidatorShouldValidateIncorrectCulture(string culture)
    {
        var result = validator.TestValidate(this.Request(culture: culture));

        if (!AvailableCultures.Contains(culture))
        {
            result.ShouldHaveValidationErrorFor(req => req.Culture)
                .WithErrorCode("CreateList.Culture.Invalid");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Culture);
        }
    }

    [Property(DisplayName = "Validator should validate default season title")]
    public void ValidatorShouldValidateDefaultSeasonTitle(string defaultSeasonOriginalTitle)
    {
        var result = validator.TestValidate(this.Request(defaultSeasonTitle: defaultSeasonOriginalTitle));

        if (String.IsNullOrWhiteSpace(defaultSeasonOriginalTitle))
        {
            result.ShouldHaveValidationErrorFor(req => req.DefaultSeasonTitle)
                .WithErrorCode("CreateList.DefaultSeasonTitle.Empty");
        } else if (defaultSeasonOriginalTitle.Length > MaxNameLength)
        {
            result.ShouldHaveValidationErrorFor(req => req.DefaultSeasonTitle)
                .WithErrorCode("CreateList.DefaultSeasonTitle.TooLong");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.DefaultSeasonTitle);
        }
    }

    [Property(DisplayName = "Validator should validate default season original title")]
    public void ValidatorShouldValidateDefaultSeasonOriginalTitle(string defaultSeasonOriginalTitle)
    {
        var result = validator.TestValidate(this.Request(defaultSeasonOriginalTitle: defaultSeasonOriginalTitle));

        if (String.IsNullOrWhiteSpace(defaultSeasonOriginalTitle))
        {
            result.ShouldHaveValidationErrorFor(req => req.DefaultSeasonOriginalTitle)
                .WithErrorCode("CreateList.DefaultSeasonOriginalTitle.Empty");
        } else if (defaultSeasonOriginalTitle.Length > MaxNameLength)
        {
            result.ShouldHaveValidationErrorFor(req => req.DefaultSeasonOriginalTitle)
                .WithErrorCode("CreateList.DefaultSeasonOriginalTitle.TooLong");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.DefaultSeasonOriginalTitle);
        }
    }

    private CreateListRequest Request(
        string name = Placeholder,
        string culture = "",
        string defaultSeasonTitle = Placeholder,
        string defaultSeasonOriginalTitle = Placeholder) =>
        new(name, culture, defaultSeasonTitle, defaultSeasonOriginalTitle);
}
