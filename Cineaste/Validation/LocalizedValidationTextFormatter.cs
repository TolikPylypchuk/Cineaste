using System;
using System.Globalization;
using System.Resources;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Formatters.Abstractions;

namespace Cineaste.Validation
{
    public sealed class LocalizedValidationTextFormatter : IValidationTextFormatter<string>
    {
        private readonly ResourceManager resourceManager;

        public LocalizedValidationTextFormatter(ResourceManager resourceManager) =>
            this.resourceManager = resourceManager;

        public string Format(ValidationText validationText) =>
            validationText.Count != 0 && !String.IsNullOrEmpty(validationText[0])
                ? this.resourceManager.GetString($"Validation{validationText[0]}", CultureInfo.CurrentUICulture)
                    ?? validationText[0]
                : String.Empty;
    }
}
