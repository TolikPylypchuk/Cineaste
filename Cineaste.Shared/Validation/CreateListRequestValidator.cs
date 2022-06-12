namespace Cineaste.Shared.Validation;

using System.Collections.Immutable;
using System.Globalization;

using static ValidationErrorCodes;

public sealed class CreateListRequestValidator : AbstractValidator<CreateListRequest>
{
    private static readonly IReadOnlySet<string> AvailableCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
        .Select(culture => culture.ToString())
        .ToImmutableHashSet();

    public CreateListRequestValidator()
    {
        this.RuleFor(req => req.Name)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(nameof(CreateListRequest.Name), Empty))
            .MaximumLength(100)
            .WithErrorCode(this.ErrorCode(nameof(CreateListRequest.Name), TooLong));

        this.RuleFor(req => req.Culture)
            .Must(AvailableCultures.Contains)
            .WithErrorCode(this.ErrorCode(nameof(CreateListRequest.Culture), Invalid));

        this.RuleFor(req => req.DefaultSeasonTitle)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(nameof(CreateListRequest.DefaultSeasonTitle), Empty))
            .MaximumLength(100)
            .WithErrorCode(this.ErrorCode(nameof(CreateListRequest.DefaultSeasonTitle), TooLong));

        this.RuleFor(req => req.DefaultSeasonOriginalTitle)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(nameof(CreateListRequest.DefaultSeasonOriginalTitle), Empty))
            .MaximumLength(100)
            .WithErrorCode(this.ErrorCode(nameof(CreateListRequest.DefaultSeasonOriginalTitle), TooLong));
    }

    private string ErrorCode(string property, string error) =>
        $"CreateList.{property}.{error}";
}
