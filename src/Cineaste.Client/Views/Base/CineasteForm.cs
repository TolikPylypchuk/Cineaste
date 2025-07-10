namespace Cineaste.Client.Views.Base;

public abstract class CineasteForm<TFormModel, TRequest, TModel, TState>
    : StatefulComponent<TState>, IValidationExecutor
    where TFormModel : FormModelBase<TRequest, TModel>
    where TRequest : IValidatable<TRequest>
{
    private IValidationExecutor? validationParent;

    [CascadingParameter(Name = CascadingParameters.ValidationExecutor)]
    public IValidationExecutor? ValidationParent
    {
        get => this.validationParent;
        set
        {
            if (!Equals(this.validationParent, value))
            {
                this.DetachValidation();

                this.validationParent = value;

                if (this.validationParent is not null)
                {
                    this.validationParent.ValidationExecuted += this.OnParentValidationExecuted;
                    this.validationParent.ValidationCleared += this.OnParentValidationCleared;
                }
            }
        }
    }

    public virtual TFormModel FormModel { get; set; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        this.ValidationResumed?.Invoke(this, EventArgs.Empty);
    }

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            this.DetachValidation();
        }

        return base.DisposeAsyncCore(disposing);
    }

    protected void RunValidation()
    {
        this.ValidationExecuted?.Invoke(this, EventArgs.Empty);
        this.StateHasChanged();
    }

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

    protected void ClearValidation()
    {
        this.ValidationCleared?.Invoke(this, EventArgs.Empty);
        this.ValidationSuspended?.Invoke(this, EventArgs.Empty);
    }

    private void OnParentValidationExecuted(object? sender, EventArgs e) =>
        this.RunValidation();

    private void OnParentValidationCleared(object? sender, EventArgs e) =>
        this.ClearValidation();

    private void DetachValidation()
    {
        if (this.validationParent is not null)
        {
            this.validationParent.ValidationExecuted -= this.OnParentValidationExecuted;
            this.validationParent.ValidationCleared -= this.OnParentValidationCleared;
        }
    }

    public event EventHandler<EventArgs>? ValidationExecuted;

    public event EventHandler<EventArgs>? ValidationCleared;

    public event EventHandler<EventArgs>? ValidationSuspended;

    public event EventHandler<EventArgs>? ValidationResumed;
}
