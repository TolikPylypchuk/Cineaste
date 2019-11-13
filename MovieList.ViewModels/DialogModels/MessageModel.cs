using ReactiveUI.Fody.Helpers;

namespace MovieList.DialogModels
{
    public sealed class MessageModel : DialogModelBase
    {
        public MessageModel(string message, string title, string? closeText = null)
            : base(message, title)
            => this.CloseText = closeText;

        [Reactive]
        public string? CloseText { get; set; }
    }
}
