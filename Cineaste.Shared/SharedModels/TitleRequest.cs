namespace Cineaste.Shared.SharedModels;

public sealed record TitleRequest(string Name, int Priority) : IValidatable<TitleRequest>
{
    public static IValidator<TitleRequest> CreateValidator() =>
        new TitleRequestValidator();
}
