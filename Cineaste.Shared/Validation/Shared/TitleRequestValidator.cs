namespace Cineaste.Shared.Validation.Shared;

using static Cineaste.Basic.Constants;

public class TitleRequestValidator : CineasteValidator<TitleRequest>
{
    public TitleRequestValidator()
        : base("Title")
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
