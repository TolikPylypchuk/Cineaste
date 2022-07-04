namespace Cineaste.Client.Validation;

using Cineaste.Client.Localization;

using Microsoft.Extensions.Localization;

public interface IValidationExecutor
{
    IStringLocalizer<Resources> Loc { get; }

    event EventHandler<RunValidationEventArgs> Validation;
}
