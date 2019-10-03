using System.Reactive;

using ReactiveUI;

namespace MovieList
{
    public static class Message
    {
        public static readonly Interaction<string, Unit> Show = new Interaction<string, Unit>();

        public static readonly Interaction<string, bool> Confirm = new Interaction<string, bool>();
    }
}
