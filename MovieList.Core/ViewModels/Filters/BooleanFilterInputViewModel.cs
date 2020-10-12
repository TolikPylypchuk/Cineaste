using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class BooleanFilterInputViewModel : FilterInput
    {
        [Reactive]
        public bool Value { get; set; }
    }
}
