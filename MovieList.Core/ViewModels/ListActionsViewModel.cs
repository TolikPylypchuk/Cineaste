using System.Collections.ObjectModel;
using System.Reactive;

using MovieList.Core.ViewModels.Filters;
using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Core.ViewModels
{
    public sealed class ListActionsViewModel : ReactiveObject
    {
        public ListActionsViewModel(ReadOnlyObservableCollection<Tag> tags)
        {
            this.Filter = new ListFilterViewModel(tags);

            this.AddNewMovie = ReactiveCommand.Create(() => { });
            this.AddNewSeries = ReactiveCommand.Create(() => { });
        }

        public ListFilterViewModel Filter { get; }

        public ReactiveCommand<Unit, Unit> AddNewMovie { get; }
        public ReactiveCommand<Unit, Unit> AddNewSeries { get; }
    }
}
