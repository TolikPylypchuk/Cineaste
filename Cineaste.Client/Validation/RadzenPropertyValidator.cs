namespace Cineaste.Client.Validation;

using Microsoft.AspNetCore.Components;

using Radzen;
using Radzen.Blazor;

public sealed class RadzenPropertyValidator<T, TProperty> : ValidatorBase
{
    public override string Text { get; set; } = String.Empty;

    [Parameter]
    public PropertyValidator<T, TProperty>? Validator { get; set; }

    protected override bool Validate(IRadzenFormComponent component)
    {
        if (component.GetValue() is TProperty value && this.Validator is not null)
        {
            var result = this.Validator.Validate(value);
            this.Text = result.Any() ? result.Aggregate((acc, item) => $"{acc}; {item}") : String.Empty;
            return !result.Any();
        }

        return true;
    }
}
