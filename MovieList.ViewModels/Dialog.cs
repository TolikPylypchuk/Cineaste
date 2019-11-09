using System.Reactive;

using MovieList.DialogModels;

using ReactiveUI;

namespace MovieList
{
    public static class Dialog
    {
        public static readonly Interaction<MessageModel, Unit> Show =
            new Interaction<MessageModel, Unit>();

        public static readonly Interaction<ConfirmationModel, bool> Confirm =
            new Interaction<ConfirmationModel, bool>();

        public static readonly Interaction<string, Unit> ShowSimple =
            new Interaction<string, Unit>();

        public static readonly Interaction<string, bool> ConfirmSimple =
            new Interaction<string, bool>();

        public static readonly Interaction<Unit, string?> CreateList = new Interaction<Unit, string?>();

        public static readonly Interaction<string, string?> SaveFile = new Interaction<string, string?>();

        public static readonly Interaction<Unit, string?> OpenFile = new Interaction<Unit, string?>();
    }
}
