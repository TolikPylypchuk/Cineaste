namespace Cineaste.Client.Views.Base;

using Cineaste.Client.Localization;

using Fluxor.Blazor.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public abstract class CineasteComponent : FluxorComponent
{
    [Inject]
    public required IStringLocalizer<Resources> Loc { get; init; }

    [Inject]
    public required IPageNavigator PageNavigator { get; init; }
}
