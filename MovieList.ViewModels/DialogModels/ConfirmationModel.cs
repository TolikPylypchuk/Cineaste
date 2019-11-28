using ReactiveUI.Fody.Helpers;

namespace MovieList.DialogModels
{
    public sealed class ConfirmationModel : DialogModelBase
    {
        public const string MessageSuffix = "Question";
        public const string TitleSuffix = "Title";

        public ConfirmationModel(string messageAndTitle)
            : this(messageAndTitle + MessageSuffix, messageAndTitle + TitleSuffix)
        { }

        public ConfirmationModel(
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
    }
}
