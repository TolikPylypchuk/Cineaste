namespace Cineaste.Client.Views.Base;

using Cineaste.Client.Localization;

using Fluxor.Blazor.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

using Radzen;

public abstract class CineasteComponent : FluxorComponent
{
    protected const int ShortNotificationDuration = 2000;

    [Inject]
    public required IStringLocalizer<Resources> Loc { get; init; }

    [Inject]
    public required IPageNavigator PageNavigator { get; init; }

    [Inject]
    public required DialogService DialogService { get; init; }

    [Inject]
    public required NotificationService NotificationService { get; init; }

    [Inject]
    public required TooltipService TooltipService { get; init; }

    protected void ShowSuccessNotification(string text, int duration) =>
        this.NotificationService.Notify(new()
        {
            Summary = this.Loc[text],
            Severity = NotificationSeverity.Success,
            Duration = duration
        });
}
