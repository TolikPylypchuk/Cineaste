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

    [Property(DisplayName = "Validator should validate that titles have distinct sequence numbers")]
    public void ValidatorShouldValidateTitlesDistinctSequenceNumbers(NonEmptyString title1, NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            titles: [title1.Get, title2.Get], differentTitleSequenceNumbers: false));

        result.ShouldHaveValidationErrorFor(req => req.Titles)
            .WithErrorCode("Franchise.Titles.Distinct.SequenceNumbers");
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

    [Property(DisplayName = "Validator should validate that original titles have distinct sequence numbers")]
    public void ValidatorShouldValidateOriginalTitlesDistinctSequenceNumbers(
        NonEmptyString title1,
        NonEmptyString title2)
    {
        var result = validator.TestValidate(this.Request(
            originalTitles: [title1.Get, title2.Get], differentOriginalTitleSequenceNumbers: false));

        result.ShouldHaveValidationErrorFor(req => req.OriginalTitles)
            .WithErrorCode("Franchise.OriginalTitles.Distinct.SequenceNumbers");
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
            items: [new FranchiseItemRequest(Guid.CreateVersion7(), type, 1, true)]));

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
            .Select(num => new FranchiseItemRequest(Guid.CreateVersion7(), FranchiseItemType.Movie, num, true))
            .ToList();

        var items2 = Enumerable.Range(1, 9)
            .Where(num => num % 2 == 0)
            .Select(num => new FranchiseItemRequest(Guid.CreateVersion7(), FranchiseItemType.Movie, num, true))
            .ToList();

        var result1 = validator.TestValidate(this.Request(items: items1));
        var result2 = validator.TestValidate(this.Request(items: items2));

        result1.ShouldNotHaveAnyValidationErrors();
        result2.ShouldHaveValidationErrorFor(req => req.Items)
            .WithErrorCode("Franchise.Items.Sequence.Invalid");
    }

    private FranchiseRequest Request(
        IEnumerable<string>? titles = null,
        bool differentTitleSequenceNumbers = true,
        IEnumerable<string>? originalTitles = null,
        bool differentOriginalTitleSequenceNumbers = true,
        IEnumerable<FranchiseItemRequest>? items = null,
        Guid kindId = default,
        FranchiseKindSource kindSource = FranchiseKindSource.Movie,
        bool showTitles = false,
        bool isLooselyConnected = true,
        bool continueNumbering = false) =>
        new(
            TitleRequests(titles, differentTitleSequenceNumbers),
            TitleRequests(originalTitles, differentOriginalTitleSequenceNumbers),
            items?.ToImmutableList().AsValue()
                ?? ImmutableList.Create(
                    new FranchiseItemRequest(Guid.CreateVersion7(), FranchiseItemType.Movie, 1, true)).AsValue(),
            kindId,
            kindSource,
            showTitles,
            isLooselyConnected,
            continueNumbering,
            null);
}
