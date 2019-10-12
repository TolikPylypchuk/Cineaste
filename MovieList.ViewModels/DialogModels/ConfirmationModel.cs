using MovieList.Properties;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.DialogModels
{
    public sealed class ConfirmationModel : ReactiveObject
    {
        public ConfirmationModel(
            string message,
            string? confirmButtonText = null,
            string? cancelButtonText = null)
        {
            this.Message = message;
            this.ConfirmButtonText = confirmButtonText ?? Resources.Confirm;
            this.CancelButtonText = cancelButtonText ?? Resources.Cancel;
        }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string ConfirmButtonText { get; set; }

        [Reactive]
        public string CancelButtonText { get; set; }
    }
}
