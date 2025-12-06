using Microsoft.Extensions.Localization;

namespace Cineaste.Client.Problems;

public interface IProblemLocalizer
{
    LocalizedString this[string problemType] { get; }
}

public sealed class ProblemLocalizer(IStringLocalizer<Localization.Problems> loc) : IProblemLocalizer
{
    public LocalizedString this[string problemType] => loc[problemType];
}
