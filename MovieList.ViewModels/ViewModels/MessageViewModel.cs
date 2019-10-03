using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.ViewModels
{
    public class MessageViewModel : ReactiveObject
    {
        public MessageViewModel(string message, string buttonText)
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
