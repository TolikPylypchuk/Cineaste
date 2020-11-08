using System.Reactive.Disposables;

using MovieList.Core.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class ListActionsControlBase : ReactiveUserControl<ListActionsViewModel> { }

    public partial class ListActionsControl : ListActionsControlBase
    {
        public ListActionsControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.AddNewMovie, v => v.AddNewMovieButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.AddNewSeries, v => v.AddNewSeriesButton)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel!, vm => vm.Search, v => v.SearchViewHost.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel!, vm => vm.Filter, v => v.FilterViewHost.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(this.ViewModel!, vm => vm.Sort, v => v.SortViewHost.ViewModel)
                    .DisposeWith(disposables);
            });
        }
    }
}
