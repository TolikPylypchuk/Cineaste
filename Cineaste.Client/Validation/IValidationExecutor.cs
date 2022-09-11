namespace Cineaste.Client.Validation;

using Cineaste.Client.Localization;

using Microsoft.Extensions.Localization;

public interface IValidationExecutor
{
    IStringLocalizer<Resources> Loc { get; }

    event EventHandler<EventArgs> ValidationExecuted;

    event EventHandler<EventArgs> ValidationCleared;

    event EventHandler<EventArgs> ValidationSuspended;

    event EventHandler<EventArgs> ValidationResumed;
}
