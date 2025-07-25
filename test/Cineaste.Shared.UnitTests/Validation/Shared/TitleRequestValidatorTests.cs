using Cineaste.Shared.Models.Shared;

namespace Cineaste.Shared.Validation.Shared;

public sealed class TitleRequestValidatorTests
{
    private const string Placeholder = "Test";

    private readonly TitleRequestValidator validator = new();

    [Property(DisplayName = "Validator should validate name")]
    public void ValidatorShouldValidateName(string name)
    {
        var result = validator.TestValidate(this.Request(name: name));

        if (String.IsNullOrWhiteSpace(name))
        {
            result.ShouldHaveValidationErrorFor(req => req.Name)
                .WithErrorCode("Title.Name.Empty");
        } else if (name.Length > MaxNameLength)
        {
            result.ShouldHaveValidationErrorFor(req => req.Name)
                .WithErrorCode("Title.Name.TooLong");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.Name);
        }
    }

    [Property(DisplayName = "Validator should validate sequence number")]
    public void ValidatorShouldValidateSequenceNumber(int sequenceNumber)
    {
        var result = validator.TestValidate(this.Request(sequenceNumber: sequenceNumber));

        if (sequenceNumber <= 0)
        {
            result.ShouldHaveValidationErrorFor(req => req.SequenceNumber)
                .WithErrorCode("Title.SequenceNumber.TooLow");
        } else
        {
            result.ShouldNotHaveValidationErrorFor(req => req.SequenceNumber);
        }
    }

    private TitleRequest Request(string name = Placeholder, int sequenceNumber = 1) =>
        new(name, sequenceNumber);
}
