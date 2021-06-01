using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class NumberFilterInputViewModel : FilterInput
    {
        public NumberFilterInputViewModel() =>
            this.WhenAnyValue(vm => vm.Number)
                .Discard()
                .Subscribe(this.inputChanged);

        [Reactive]
        public int Number { get; set; }
    }
}
