using Cineaste.Shared.Models.Franchise;

using static Cineaste.Shared.Validation.TestData.TitleUtils;

namespace Cineaste.Shared.Validation.Franchise;

public class FranchiseRequestValidatorTests
{
    private static readonly string[] SingleEmptyString = [""];

    private readonly FranchiseRequestValidator validator = new();

    [Fact(DisplayName = "Validator should validate that titles aren't empty when they are shown")]
    public void ValidatorShouldValidateTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(titles: [], showTitles: true));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("Franchise.ShowTitles.Invalid");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct names")]
    public void ValidatorShouldValidateTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(titles: [title.Get, title.Get]));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Franchise.Titles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that titles have distinct priorities")]
    public void ValidatorShouldValidateTitlesDistinctPriorities(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            titles: [title1.Get, title2.Get], differentTitlePriorities: false));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Franchise.Titles.Distinct.Priorities");
    }

    [Fact(DisplayName = "Validator should validate that original titles aren't empty when they are shown")]
    public void ValidatorShouldValidateOriginalTitlesNotEmpty()
    {
        var result = validator.TestValidate(this.Request(originalTitles: [], showTitles: true));

        result.ShouldHaveValidationErrors()
            .WithErrorCode("Franchise.ShowTitles.Invalid");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct names")]
    public void ValidatorShouldValidateOriginalTitlesDistinctNames(NonEmptyString title)
    {
        var result = validator.TestValidate(this.Request(originalTitles: [title.Get, title.Get]));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Franchise.OriginalTitles.Distinct.Names");
    }

    [Property(DisplayName = "Validator should validate that original titles have distinct priorities")]
    public void ValidatorShouldValidateOriginalTitlesDistinctPriorities(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            originalTitles: [title1.Get, title2.Get], differentOriginalTitlePriorities: false));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Franchise.OriginalTitles.Distinct.Priorities");
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

    [Property(DisplayName = "Validator should validate item types")]
    public void ValidatorShouldValidateItemTypes(int typeInt)
    {
        var type = (FranchiseItemType)typeInt;

        var result = validator.TestValidate(this.Request(
            items: [new FranchiseItemRequest(Guid.NewGuid(), type, 1, true)]));

        if (Enum.IsDefined(type))
        {
            result.ShouldNotHaveAnyValidationErrors();
        } else
        {
            result.ShouldHaveValidationErrors()
                .WithErrorCode("Franchise.Items.Type.Invalid");
        }
    }

    [Fact(DisplayName = "Validator should validate that items have correct sequence")]
    public void ValidatorShouldValidateCorrectSequence()
    {
        var items1 = Enumerable.Range(1, 9)
            .Select(num => new FranchiseItemRequest(Guid.NewGuid(), FranchiseItemType.Movie, num, true))
            .ToList();

        var items2 = Enumerable.Range(1, 9)
            .Where(num => num % 2 == 0)
            .Select(num => new FranchiseItemRequest(Guid.NewGuid(), FranchiseItemType.Movie, num, true))
            .ToList();

        var result1 = validator.TestValidate(this.Request(items: items1));
        var result2 = validator.TestValidate(this.Request(items: items2));

        result1.ShouldNotHaveAnyValidationErrors();
        result2.ShouldHaveValidationErrorFor(req => req.Items)
            .WithErrorCode("Franchise.Items.Sequence.Invalid");
    }

    private FranchiseRequest Request(
        IEnumerable<string>? titles = null,
        bool differentTitlePriorities = true,
        IEnumerable<string>? originalTitles = null,
        bool differentOriginalTitlePriorities = true,
        IEnumerable<FranchiseItemRequest>? items = null,
        bool showTitles = false,
        bool isLooselyConnected = true,
        bool continueNumbering = false) =>
        new(
            Guid.NewGuid(),
            TitleRequests(titles, differentTitlePriorities),
            TitleRequests(originalTitles, differentOriginalTitlePriorities),
            items?.ToImmutableList().AsValue()
                ?? ImmutableList.Create(
                    new FranchiseItemRequest(Guid.NewGuid(), FranchiseItemType.Movie, 1, true)).AsValue(),
            showTitles,
            isLooselyConnected,
            continueNumbering);
}
