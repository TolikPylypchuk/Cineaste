using Microsoft.Extensions.Localization;

namespace Cineaste.Client.Validation;

public interface IValidationExecutor
{
    IStringLocalizer<Resources> Loc { get; }

    event EventHandler<EventArgs> ValidationExecuted;

    event EventHandler<EventArgs> ValidationCleared;

    event EventHandler<EventArgs> ValidationSuspended;

    event EventHandler<EventArgs> ValidationResumed;
}
