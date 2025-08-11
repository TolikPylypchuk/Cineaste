namespace Cineaste.Client.Components.Base;

public abstract class ValidatableComponent : CineasteComponent, IValidationExecutor
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

    public abstract IReadOnlySet<string> ValidationErrors { get; }

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
