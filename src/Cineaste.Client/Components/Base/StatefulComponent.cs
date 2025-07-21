using Cineaste.Client.Store;

namespace Cineaste.Client.Components.Base;

public abstract class StatefulComponent<TState> : CineasteComponent
{
    [Inject]
    public required IState<TState> State { get; init; }

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
