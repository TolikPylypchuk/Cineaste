using FluentValidation.Results;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Cineaste.Shared.Validation;

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
            Substitute.For<IValidator<TestSuccessfulValidatable>>();
    }

    public class TestFailingValidatable : IValidatable<TestFailingValidatable>
    {
        public static IValidator<TestFailingValidatable> Validator
        {
            get
            {
                var validator = Substitute.For<IValidator<TestFailingValidatable>>();

                validator.Validate(Arg.Any<ValidationContext<TestFailingValidatable>>())
                    .Throws(new ValidationException([new ValidationFailure("Test", "Test")]));

                return validator;
            }
        }
    }
}
