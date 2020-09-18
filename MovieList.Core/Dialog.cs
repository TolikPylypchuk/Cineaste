using System.Reactive;

using MovieList.Core.DialogModels;
using MovieList.Core.ViewModels.Forms.Preferences;

using ReactiveUI;

namespace MovieList.Core
{
    public static class Dialog
    {
        public static readonly Interaction<MessageModel, Unit> ShowMessage = new();
        public static readonly Interaction<ConfirmationModel, bool> Confirm = new();
        public static readonly Interaction<InputModel, string?> Input = new();
        public static readonly Interaction<ColorModel, string?> ColorPicker = new();
        public static readonly Interaction<TagFormViewModel, TagFormViewModel?> TagForm = new();
        public static readonly Interaction<string, string?> SaveFile = new();
        public static readonly Interaction<Unit, string?> OpenFile = new();
        public static readonly Interaction<AboutModel, Unit> ShowAbout = new();
    }
}
