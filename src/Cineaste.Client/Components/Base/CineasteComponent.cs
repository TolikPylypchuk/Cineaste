using Cineaste.Client.Navigation;

using Fluxor.Blazor.Web.Components;

using Microsoft.Extensions.Localization;

namespace Cineaste.Client.Components.Base;

public abstract class CineasteComponent : FluxorComponent
{
    [Inject]
    public required IStringLocalizer<Resources> Loc { get; init; }

    [Inject]
    public required IPageNavigator PageNavigator { get; init; }

    [Inject]
    public required IDispatcher Dispatcher { get; init; }
}
