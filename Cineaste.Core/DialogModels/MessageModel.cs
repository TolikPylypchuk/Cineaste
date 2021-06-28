using System.Reactive;

using ReactiveUI.Fody.Helpers;

namespace Cineaste.Core.DialogModels
{
    public sealed class MessageModel : DialogModelBase<Unit>
    {
        public MessageModel(string message, string title, string? closeText = null)
            : base(message, title) =>
            this.CloseText = closeText;

        [Reactive]
        public string? CloseText { get; set; }
    }
}
