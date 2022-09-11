namespace Cineaste.Client.Views.Base;

using System;

using Cineaste.Client.Localization;

using Fluxor.Blazor.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

using Radzen;

public abstract class CineasteComponent : FluxorComponent
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

    protected void ShowSuccessNotification(string text, int duration) =>
        this.NotificationService.Notify(new()
        {
            Summary = this.Loc[text],
            Severity = NotificationSeverity.Success,
            Duration = duration
        });
}
