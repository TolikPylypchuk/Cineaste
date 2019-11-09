using System.Reactive;

using ReactiveUI;

namespace MovieList
{
    public static class Dialog
    {
        public static readonly Interaction<string, Unit> Show =
            new Interaction<string, Unit>();

        public static readonly Interaction<string, bool> Confirm =
            new Interaction<string, bool>();

        public static readonly Interaction<string, string?> Input = new Interaction<string, string?>();

        public static readonly Interaction<string, string?> SaveFile = new Interaction<string, string?>();

        public static readonly Interaction<Unit, string?> OpenFile = new Interaction<Unit, string?>();
    }
}
