using MovieList.Properties;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.DialogModels
{
    public sealed class MessageModel : ReactiveObject
    {
        public MessageModel(string message, string? buttonText = null)
        {
            this.Message = message;
            this.ButtonText = buttonText ?? Resources.OK;
        }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string ButtonText { get; set; }
    }
}
