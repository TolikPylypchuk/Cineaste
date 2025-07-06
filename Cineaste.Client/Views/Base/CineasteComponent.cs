using Fluxor.Blazor.Web.Components;

using Microsoft.Extensions.Localization;

namespace Cineaste.Client.Views.Base;

public abstract class CineasteComponent : FluxorComponent
{
    [Inject]
    public required IStringLocalizer<Resources> Loc { get; init; }

    [Inject]
    public required IPageNavigator PageNavigator { get; init; }
}
