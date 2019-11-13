using System;

using ReactiveUI.Fody.Helpers;

namespace MovieList.DialogModels
{
    public class InputModel : DialogModelBase
    {
        public InputModel(
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
        public string Value { get; set; } = String.Empty;
    }
}
