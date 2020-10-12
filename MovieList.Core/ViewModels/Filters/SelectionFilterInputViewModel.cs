using System.Collections.ObjectModel;

using ReactiveUI.Fody.Helpers;

namespace MovieList.Core.ViewModels.Filters
{
    public sealed class SelectionFilterInputViewModel : FilterInput
    {
        public SelectionFilterInputViewModel(ReadOnlyObservableCollection<string> elements)
            => this.Elements = elements;

        public ReadOnlyObservableCollection<string> Elements { get; }

        [Reactive]
        public string? SelectedElement { get; set; }
    }
}
