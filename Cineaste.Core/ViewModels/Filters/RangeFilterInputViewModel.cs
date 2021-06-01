using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Cineaste.Core.ViewModels.Filters
{
    public sealed class RangeFilterInputViewModel : FilterInput
    {
        public RangeFilterInputViewModel() =>
            this.WhenAnyValue(vm => vm.Start, vm => vm.End)
                .Discard()
                .Subscribe(this.inputChanged);

        [Reactive]
        public int Start { get; set; }

        [Reactive]
        public int End { get; set; }
    }
}
