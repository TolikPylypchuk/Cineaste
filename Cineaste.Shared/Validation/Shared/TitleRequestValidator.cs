using static Cineaste.Basic.Constants;

namespace Cineaste.Shared.Validation.Shared;

public class TitleRequestValidator : CineasteValidator<TitleRequest>
{
    public TitleRequestValidator(string? prefix = null)
        : base(prefix is not null ? prefix : "Title")
    {
        this.RuleFor(req => req.Name)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Name, Empty))
            .MaximumLength(MaxNameLength)
            .WithErrorCode(this.ErrorCode(req => req.Name, TooLong));

        this.RuleFor(req => req.Priority)
            .GreaterThan(0)
            .WithErrorCode(this.ErrorCode(req => req.Priority, TooLow));
    }
}
