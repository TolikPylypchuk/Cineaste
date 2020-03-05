using System.Reactive;

using ReactiveUI;

namespace MovieList.ViewModels
{
    public sealed class NewItemViewModel : ReactiveObject
    {
        public NewItemViewModel()
        {
            this.AddNewMovie = ReactiveCommand.Create(() => { });
            this.AddNewSeries = ReactiveCommand.Create(() => { });
        }

        public ReactiveCommand<Unit, Unit> AddNewMovie { get; }
        public ReactiveCommand<Unit, Unit> AddNewSeries { get; }
    }
}
