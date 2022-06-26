namespace Cineaste.Shared.Validation;

public class TitleRequestValidator : CineasteValidator<TitleRequest>
{
    public TitleRequestValidator()
        : base("Title")
    {
        this.RuleFor(req => req.Name)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Name, Empty))
            .MaximumLength(100)
            .WithErrorCode(this.ErrorCode(req => req.Name, TooLong));

        this.RuleFor(req => req.Priority)
            .GreaterThan(0)
            .WithErrorCode(this.ErrorCode(req => req.Priority, TooLow));
    }
}
