namespace Cineaste.Client.Validation;

using System.Collections.Specialized;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

public sealed class FormValidator<TRequest, TProperty> : ComponentBase
{
    private TProperty? value;
    private object? trigger;
    private bool isInitialized = false;
    private bool isSuspended = false;

    [Parameter]
    public TProperty? Value
    {
        get => this.value;
        set
        {
            if (!Equals(this.value, value))
            {
                this.value = value;
                this.Validate();
            }
        }
    }

    [Parameter]
    public object? Trigger
    {
        get => this.trigger;
        set
        {
            if (!Equals(this.trigger, value))
            {
                this.trigger = value;
                this.Validate();
            }
        }
    }

    [CascadingParameter]
    public Func<TRequest?> Request { get; set; } = null!;

    [Parameter]
    public PropertyValidator<TRequest, TProperty?>? Validator { get; set; }

    [Parameter]
    public string Class { get; set; } = String.Empty;

    [Parameter]
    public string Style { get; set; } = String.Empty;

    [Parameter]
    public bool Popup { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = new();

    public bool IsValid { get; private set; } = true;

    public string Text { get; private set; } = String.Empty;

    public void Validate()
    {
        if (!this.isInitialized || this.isSuspended)
        {
            return;
        }

        if (this.Validator is not null && this.Request() is { } request)
        {
            var result = this.Validator.Validate(request, this.Value);
            this.Text = result.Any() ? result.Distinct().Aggregate((acc, item) => $"{acc}; {item}") : String.Empty;
        } else
        {
            this.Text = String.Empty;
        }

        this.IsValid = String.IsNullOrEmpty(this.Text);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this.Validator is { Executor: { } executor })
        {
            executor.ValidationExecuted += (sender, e) =>
            {
                this.Validate();

                if (!this.IsValid)
                {
                    e.ValidationFailed();
                }
            };

            executor.ValidationCleared += (sender, e) =>
            {
                this.Text = String.Empty;
                this.IsValid = true;
            };

            executor.ValidationSuspended += (sender, e) => this.isSuspended = true;
            executor.ValidationResumed += (sender, e) => this.isSuspended = false;
        }

        this.isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!this.IsValid)
        {
            var @class = $"rz-message rz-messages-error {(this.Popup ? "rz-message-popup" : "")} {this.Class}";

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "style", this.Style);
            builder.AddAttribute(2, "class", @class);
            builder.AddMultipleAttributes(3, this.Attributes);
            builder.AddContent(4, this.Text);
            builder.CloseElement();
        }
    }

    private void Validate(object? sender, NotifyCollectionChangedEventArgs e) =>
        this.Validate();
}
