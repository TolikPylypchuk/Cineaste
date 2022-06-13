namespace Cineaste.Client.Services.Validation;

using Cineaste.Client.Localization;

using Microsoft.Extensions.Localization;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Formatters.Abstractions;

[AutoConstructor]
public sealed partial class LocalizedValidationTextFormatter : IValidationTextFormatter<string>
{
    private readonly IStringLocalizer<Resources> loc;

    public string Format(ValidationText validationText) =>
        validationText.Any()
            ? validationText.Select(text => this.loc[text].ToString()).Aggregate((acc, text) => $"{acc}; {text}")
            : String.Empty;
}
