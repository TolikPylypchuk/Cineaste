using Cineaste.Client.Store;

namespace Cineaste.Client.Components.Base;

public abstract class CineasteForm<TFormModel, TRequest, TModel, TState> : ValidatableComponent
    where TFormModel : FormModelBase<TRequest, TModel>
    where TRequest : IValidatable<TRequest>
    where TModel : IIdentifyableModel
{
    [Inject]
    public required IState<TState> State { get; init; }

    public virtual TFormModel FormModel { get; set; } = null!;

    public override IReadOnlySet<string> ValidationErrors =>
        this.FormModel.ValidationErrors;

    protected void SubsribeToSuccessfulResult<TAction>(Action action)
        where TAction : ResultAction =>
        this.SubscribeToAction<TAction>(result =>
        {
            if (result.IsSuccessful)
            {
                action();
            }
        });

    protected void WithValidation(Action<TRequest> action)
    {
        this.RunValidation();

        if (this.FormModel.ValidationErrors.Count == 0)
        {
            action(this.FormModel.Request!);
        }
    }

    protected async Task WithValidation(Func<TRequest, Task> action)
    {
        this.RunValidation();

        if (this.FormModel.ValidationErrors.Count == 0)
        {
            await action(this.FormModel.Request!);
        }
    }
}
