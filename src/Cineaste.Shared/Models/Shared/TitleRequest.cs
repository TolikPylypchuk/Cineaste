using Cineaste.Shared.Validation.Shared;

namespace Cineaste.Shared.Models.Shared;

public sealed record TitleRequest(string Name, int Priority) : IValidatable<TitleRequest>
{
    public static IValidator<TitleRequest> Validator { get; } = new TitleRequestValidator();
}
