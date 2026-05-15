namespace Cineaste.Client.Components.Base;

public abstract class StatefulCineasteForm<TFormModel, TRequest, TModel, TState>
    : CineasteForm<TFormModel, TRequest, TModel>
    where TFormModel : FormModelBase<TRequest, TModel>
    where TRequest : IValidatable<TRequest>
{
    [Inject]
    public required IState<TState> State { get; init; }

}
