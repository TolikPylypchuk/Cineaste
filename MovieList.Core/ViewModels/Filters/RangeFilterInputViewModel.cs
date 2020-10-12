using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class RangeFilterInputViewModel : FilterInput
    {
        [Reactive]
        public int Start { get; set; }

        [Reactive]
        public int End { get; set; }
    }
}
