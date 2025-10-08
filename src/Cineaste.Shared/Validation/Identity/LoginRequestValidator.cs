namespace Cineaste.Shared.Validation.Identity;

public sealed class LoginRequestValidator : CineasteValidator<LoginRequest>
{
    public LoginRequestValidator()
        : base("Login")
    {
        this.RuleFor(req => req.Email)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Email, Empty));

        this.RuleFor(req => req.Email)
            .EmailAddress()
            .WithErrorCode(this.ErrorCode(req => req.Email, Invalid));

        this.RuleFor(req => req.Password)
            .NotEmpty()
            .WithErrorCode(this.ErrorCode(req => req.Password, Empty));
    }
}
