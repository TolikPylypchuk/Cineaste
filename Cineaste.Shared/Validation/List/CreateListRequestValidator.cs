namespace Cineaste.Shared.Validation.List;

using System.Collections.Immutable;
using System.Globalization;

public sealed class CreateListRequestValidator : CineasteValidator<CreateListRequest>
{
    private static readonly IReadOnlySet<string> AvailableCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
        .Select(culture => culture.ToString())
        .ToImmutableHashSet();

    public CreateListRequestValidator()
        : base("CreateList")
    {
        this.RuleFor(req => req.Name)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Name, Empty))
            .MaximumLength(MaxNameLength)
            .WithErrorCode(this.ErrorCode(req => req.Name, TooLong));

        this.RuleFor(req => req.Culture)
            .Must(AvailableCultures.Contains)
            .WithErrorCode(this.ErrorCode(req => req.Culture, Invalid));

        this.RuleFor(req => req.DefaultSeasonTitle)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.DefaultSeasonTitle, Empty))
            .MaximumLength(MaxNameLength)
            .WithErrorCode(this.ErrorCode(req => req.DefaultSeasonTitle, TooLong));

        this.RuleFor(req => req.DefaultSeasonOriginalTitle)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.DefaultSeasonOriginalTitle, Empty))
            .MaximumLength(MaxNameLength)
            .WithErrorCode(this.ErrorCode(req => req.DefaultSeasonOriginalTitle, TooLong));
    }
}