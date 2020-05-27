using System.Reactive;

using MovieList.DialogModels;

using ReactiveUI;

namespace MovieList
{
    public static class Dialog
    {
        public static readonly Interaction<MessageModel, Unit> ShowMessage =
            new Interaction<MessageModel, Unit>();

        public static readonly Interaction<ConfirmationModel, bool> Confirm =
            new Interaction<ConfirmationModel, bool>();

        public static readonly Interaction<InputModel, string?> Input = new Interaction<InputModel, string?>();

        public static readonly Interaction<ColorModel, string?> ColorPicker = new Interaction<ColorModel, string?>();

        public static readonly Interaction<string, string?> SaveFile = new Interaction<string, string?>();

        public static readonly Interaction<Unit, string?> OpenFile = new Interaction<Unit, string?>();
    }
}
