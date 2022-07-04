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

    public bool RunValidation()
    {
        var args = new RunValidationEventArgs();
        this.Validation?.Invoke(this, args);

        this.StateHasChanged();

        return args.IsSuccessful;
    }

    public void WithValidation(Action action)
    {
        bool success = this.RunValidation();

        if (success)
        {
            action();
        }
    }

    public event EventHandler<RunValidationEventArgs>? Validation;
}
