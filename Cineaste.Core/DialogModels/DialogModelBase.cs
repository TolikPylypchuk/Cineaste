using System.Reactive;

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

            this.Close = ReactiveCommand.Create(() => { });
        }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string Title { get; set; }

        public ReactiveCommand<Unit, Unit> Close { get; }
    }
}
