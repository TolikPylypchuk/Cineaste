using System.Reactive.Disposables;

using MovieList.Core.ViewModels;

using ReactiveUI;

namespace MovieList.Views
{
    public abstract class ListSearchControlBase : ReactiveUserControl<ListSearchViewModel> { }

    public partial class ListSearchControl : ListSearchControlBase
    {
        public ListSearchControl()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.ViewModel)
                    .BindTo(this, v => v.DataContext)
                    ?.DisposeWith(disposables);

                this.Bind(this.ViewModel, vm => vm.FilterItem, v => v.FilterItemViewHost.ViewModel)
                    ?.DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.FindNext, v => v.FindNextButton)
                    ?.DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.FindPrevious, v => v.FindPreviousButton)
                    ?.DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.StopSearch, v => v.StopSearchButton)
                    ?.DisposeWith(disposables);

                this.BindCommand(this.ViewModel!, vm => vm.Clear, v => v.ClearSearchButton)
                    ?.DisposeWith(disposables);
            });
        }
    }
}
