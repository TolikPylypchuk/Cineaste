namespace Cineaste.Client.Views.Base;

using System;

using Cineaste.Client.Localization;
using Cineaste.Client.Validation;

using Fluxor.Blazor.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

using Radzen;

public abstract class CineasteComponent : FluxorComponent, IValidationExecutor
{
    protected const int ShortNotificationDuration = 2000;

    [Inject]
    public IStringLocalizer<Resources> Loc { get; private set; } = null!;

    [Inject]
    protected IPageNavigator PageNavigator { get; private set; } = null!;

    [Inject]
    protected DialogService DialogService { get; private set; } = null!;

    [Inject]
    protected NotificationService NotificationService { get; private set; } = null!;

    [Inject]
    protected TooltipService TooltipService { get; private set; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        this.ValidationResumed?.Invoke(this, EventArgs.Empty);
    }

    protected bool RunValidation()
    {
        var args = new ExecuteValidationEventArgs();
        this.ValidationExecuted?.Invoke(this, args);

        this.StateHasChanged();

        return args.IsSuccessful;
    }

    protected void WithValidation(Action action)
    {
        bool success = this.RunValidation();

        if (success)
        {
            action();
        }
    }

    protected async Task WithValidation(Func<Task> action)
    {
        bool success = this.RunValidation();

        if (success)
        {
            await action();
        }
    }

    protected void ClearValidation()
    {
        this.ValidationCleared?.Invoke(this, EventArgs.Empty);
        this.ValidationSuspended?.Invoke(this, EventArgs.Empty);
    }

    protected void AttachValidationTo(IValidationExecutor parent)
    {
        parent.ValidationExecuted += (sender, e) =>
        {
            bool success = this.RunValidation();

            if (!success)
            {
                e.ValidationFailed();
            }
        };

        parent.ValidationCleared += (sender, e) => this.ClearValidation();
    }

    protected void ShowSuccessNotification(string text, int duration) =>
        this.NotificationService.Notify(new()
        {
            Summary = this.Loc[text],
            Severity = NotificationSeverity.Success,
            Duration = duration
        });

    public event EventHandler<ExecuteValidationEventArgs>? ValidationExecuted;

    public event EventHandler<EventArgs>? ValidationCleared;

    public event EventHandler<EventArgs>? ValidationSuspended;

    public event EventHandler<EventArgs>? ValidationResumed;
}
