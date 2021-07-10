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
            this.resourceManager.GetString($"Validation{validationText[0]}", CultureInfo.CurrentUICulture) 
                ?? validationText[0];
    }
}
