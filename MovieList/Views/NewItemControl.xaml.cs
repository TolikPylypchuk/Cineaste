using System.Reactive.Disposables;

using MovieList.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class NewItemControlBase : ReactiveUserControl<NewItemViewModel> { }

    public partial class NewItemControl : NewItemControlBase
    {
        public NewItemControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.AddNewMovie, v => v.AddNewMovieButton)
                    .DisposeWith(disposables);

                this.BindCommand(this.ViewModel, vm => vm.AddNewSeries, v => v.AddNewSeriesButton)
                    .DisposeWith(disposables);
            });
        }
    }
}
