namespace Cineaste.Client.Views.Base;

using Cineaste.Client.Store;

using Microsoft.AspNetCore.Components;

public abstract class StatefulComponent<TState> : CineasteComponent
{
    [Inject]
    protected IState<TState> State { get; private set; } = null!;

    [Inject]
    protected IDispatcher Dispatcher { get; private set; } = null!;

    protected void SubsribeToSuccessfulResult<TAction>(Action action)
        where TAction : ResultAction =>
        this.SubscribeToAction<TAction>(result =>
        {
            if (result.IsSuccessful)
            {
                action();
            }
        });
}
