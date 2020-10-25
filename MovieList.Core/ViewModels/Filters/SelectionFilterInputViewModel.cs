using System.Collections.ObjectModel;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class SelectionFilterInputViewModel : FilterInput
    {
        public SelectionFilterInputViewModel(ReadOnlyObservableCollection<string> items)
        {
            this.Items = items;

            this.WhenAnyValue(vm => vm.SelectedItem)
                .Discard()
                .Subscribe(this.inputChanged);
        }

        public ReadOnlyObservableCollection<string> Items { get; }

        [Reactive]
        public string? SelectedItem { get; set; }
    }
}
