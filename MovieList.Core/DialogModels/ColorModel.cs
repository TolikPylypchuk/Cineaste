using System;

using ReactiveUI.Fody.Helpers;

namespace MovieList.DialogModels
{
    public sealed class ColorModel : DialogModelBase
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
        }

        [Reactive]
        public string? ConfirmText { get; set; }

        [Reactive]
        public string? CancelText { get; set; }

        [Reactive]
        public string Color { get; set; } = String.Empty;
    }
}
