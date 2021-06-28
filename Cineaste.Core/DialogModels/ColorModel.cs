using System;

using Cineaste.Core.Validation;

using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace Cineaste.Core.DialogModels
{
    public sealed class ColorModel : DialogModelBase<string?>, IValidatableViewModel
    {
        public ColorModel(
            string message,
            string title,
            string? confirmButtonText = null,
            string? cancelButtonText = null)
            : base(message, title)
        {
            this.ConfirmText = confirmButtonText;
            this.CancelText = cancelButtonText;

            this.ValidationContext = new ValidationContext();

            this.ColorRule = this.ValidationRule(vm => vm.Color, HexColorValidator.IsArgbString, "HexColorInvalid");
        }

        [Reactive]
        public string? ConfirmText { get; set; }

        [Reactive]
        public string? CancelText { get; set; }

        [Reactive]
        public string Color { get; set; } = String.Empty;

        public ValidationHelper ColorRule { get; }

        public ValidationContext ValidationContext { get; }
    }
}
