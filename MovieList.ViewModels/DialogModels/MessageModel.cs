using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.DialogModels
{
    public sealed class MessageModel : ReactiveObject
    {
        public MessageModel(string message, string buttonText)
        {
            this.Message = message;
            this.ButtonText = buttonText;
        }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string ButtonText { get; set; }
    }
}
