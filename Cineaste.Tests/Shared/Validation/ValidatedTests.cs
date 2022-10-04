namespace Cineaste.Shared.Validation;

using FluentValidation;
using FluentValidation.Results;

public sealed class ValidatedTests
{
    [Fact(DisplayName = "Validated should be created for valid objects")]
    public void ValidatedShouldBeCreatedForValidObjects()
    {
        var target = new TestSuccessfulValidatable();
        var validated = target.Validated();
        Assert.Equal(validated.Value, target);
    }

    [Fact(DisplayName = "Validated should throw for invalid objects")]
    public void ValidatedShouldThrowForInvalidObjects()
    {
        var target = new TestFailingValidatable();
        Assert.Throws<ValidationException>(target.Validated);
    }

    [Fact(DisplayName = "Validated should be null for null objects")]
    public void ValidatedShouldBeNullForNullObjects() =>
        Assert.Null(Validated<TestSuccessfulValidatable>.Create(null));

    public class TestSuccessfulValidatable : IValidatable<TestSuccessfulValidatable>
    {
        public static IValidator<TestSuccessfulValidatable> Validator =>
            new Mock<IValidator<TestSuccessfulValidatable>>().Object;
    }

    public class TestFailingValidatable : IValidatable<TestFailingValidatable>
    {
        public static IValidator<TestFailingValidatable> Validator
        {
            get
            {
                var mock = new Mock<IValidator<TestFailingValidatable>>();

                mock.Setup(m => m.Validate(It.IsAny<ValidationContext<TestFailingValidatable>>()))
                    .Throws(new ValidationException(new[] { new ValidationFailure("Test", "Test") }));

                return mock.Object;
            }
        }
    }
}
