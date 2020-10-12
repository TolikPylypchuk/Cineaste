using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class NumberFilterInputViewModel : FilterInput
    {
        [Reactive]
        public int Number { get; set; }
    }
}
