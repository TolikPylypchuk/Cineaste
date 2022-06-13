namespace Cineaste.Client.Validation;

using Microsoft.AspNetCore.Components;

using Radzen;
using Radzen.Blazor;

using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.States;

public sealed partial class ReactiveValidator : ValidatorBase
{
    private readonly BehaviorSubject<IValidationState> validationState = new(ValidationState.Valid);
    private ValidationHelper? rule;
    private IDisposable? validationStateSubscription;

    public override string Text { get; set; } = String.Empty;

    [Inject]
    public IValidationTextFormatter<string> Formatter { get; set; } = null!;

    [Parameter]
    public ValidationHelper? Rule
    {
        get => this.rule;
        set
        {
            this.rule = value;

            if (this.rule is not null)
            {
                this.validationStateSubscription?.Dispose();
                this.validationState.OnNext(ValidationState.Valid);
                this.validationStateSubscription = this.rule.ValidationChanged.Subscribe(this.validationState);
            }
        }
    }

    protected override bool Validate(IRadzenFormComponent component)
    {
        var currentState = this.validationState.Value;

        this.Text = this.Formatter.Format(currentState.Text);

        return currentState.IsValid;
    }
}
