using Microsoft.AspNetCore.Components;

namespace Cineaste.Client.Views.Base;

public abstract class StatefulComponent<TState> : CineasteComponent
{
    [Inject]
    protected IState<TState> State { get; private set; } = null!;

    [Inject]
    protected IDispatcher Dispatcher { get; private set; } = null!;
}
