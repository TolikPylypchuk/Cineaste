using System;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.DialogModels
{
    public class InputModel : ReactiveObject
    {
        public InputModel(string message, string confirmButtonText, string cancelButtonText)
        {
            this.Message = message;
            this.ConfirmButtonText = confirmButtonText;
            this.CancelButtonText = cancelButtonText;
        }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string ConfirmButtonText { get; set; }

        [Reactive]
        public string CancelButtonText { get; set; }

        [Reactive]
        public string Value { get; set; } = String.Empty;
    }
}
