using System.Collections.ObjectModel;
using System.Reactive;

using MovieList.Core.ViewModels.Filters;
using MovieList.Data;
using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels
{
    public sealed class ListActionsViewModel : ReactiveObject
    {
        public ListActionsViewModel(
            ReadOnlyObservableCollection<ListItemViewModel> listItems,
            ReadOnlyObservableCollection<Kind> kinds,
            ReadOnlyObservableCollection<Tag> tags,
            Settings settings)
        {
            this.Search = new ListSearchViewModel(listItems, kinds, tags);
            this.Filter = new ListFilterViewModel(kinds, tags);
            this.Sort = new ListSortViewModel(settings);

            this.AddNewMovie = ReactiveCommand.Create(() => { });
            this.AddNewSeries = ReactiveCommand.Create(() => { });
        }

        public ListSearchViewModel Search { get; }
        public ListFilterViewModel Filter { get; }
        public ListSortViewModel Sort { get; }

        public ReactiveCommand<Unit, Unit> AddNewMovie { get; }
        public ReactiveCommand<Unit, Unit> AddNewSeries { get; }
    }
}
