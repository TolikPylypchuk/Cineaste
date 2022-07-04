namespace Cineaste.Client.Validation;

using System.Collections.Specialized;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

public sealed class FormValidator<T, TValue> : ComponentBase
{
    private TValue? value;
    private bool settingValueFirstTime = true;

    [Parameter]
    public TValue? Value
    {
        get => this.value;
        set
        {
            if (Equals(this.value, value))
            {
                return;
            }

            this.value = value;

            if (!this.settingValueFirstTime)
            {
                this.Validate();
            }

            this.settingValueFirstTime = false;
        }
    }

    [Parameter]
    public PropertyValidator<T, TValue?>? Validator { get; set; }

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
        if (this.Validator is not null)
        {
            var result = this.Validator.Validate(this.Value);
            this.Text = result.Any() ? result.Aggregate((acc, item) => $"{acc}; {item}") : String.Empty;
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
            executor.Validation += (sender, e) =>
            {
                this.Validate();

                if (!this.IsValid)
                {
                    e.ValidationFailed();
                }
            };
        }
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
