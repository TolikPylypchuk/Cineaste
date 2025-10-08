using Cineaste.Shared.Validation.Identity;

namespace Cineaste.Shared.Models.Identity;

public sealed record LoginRequest(string Email, string Password) : IValidatable<LoginRequest>
{
    public static IValidator<LoginRequest> Validator { get; } = new LoginRequestValidator();
}
