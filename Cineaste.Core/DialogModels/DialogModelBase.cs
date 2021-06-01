using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Cineaste.Core.DialogModels
{
    public abstract class DialogModelBase : ReactiveObject
    {
        protected DialogModelBase(string message, string title)
        {
            this.Message = message;
            this.Title = title;
        }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string Title { get; set; }
    }
}
